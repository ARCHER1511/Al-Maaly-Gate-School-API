using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAppUserRepository _userRepo;
        private readonly IAppRoleRepository _roleRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly ITeacherRepository _teacherRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IParentRepository _parentRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthenticationService(
            IAppUserRepository userRepo,
            IAppRoleRepository roleRepo,
            IAdminRepository adminRepo,
            ITeacherRepository teacherRepo,
            IStudentRepository studentRepo,
            IParentRepository parentRepo,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMapper mapper
        )
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _adminRepo = adminRepo;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _parentRepo = parentRepo;
            _unitOfWork = unitOfWork;
            _config = config;
            _mapper = mapper;
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return ServiceResult<AuthResponse>.Fail("Email already registered");

            var user = _mapper.Map<AppUser>(request);
            //Ensure Id is set
            if (string.IsNullOrEmpty(user.Id))
                user.Id = Guid.NewGuid().ToString();

            var result = await _userRepo.CreateAsync(user, request.Password);

            if (!result.Succeeded) 
            {
                var errorMessages = string.Join(", ",result.Errors.Select(e => e.Description));
                return ServiceResult<AuthResponse>.Fail($"User creation failed: {errorMessages}");
            }

            if (!string.IsNullOrEmpty(request.Role))
                await _userRepo.AddToRoleAsync(user, request.Role);
            switch (request.Role.ToLower()) 
            {
                
                case "admin":
                    var admin = _mapper.Map<Admin>(request);
                    admin.AppUserId = user.Id;
                    await _adminRepo.AddAsync(admin);
                    await _unitOfWork.SaveChangesAsync();
                    break;

                case "teacher":
                    var teacher = _mapper.Map<Teacher>(request);
                    teacher.AppUserId = user.Id;
                    await _teacherRepo.AddAsync(teacher);
                    await _unitOfWork.SaveChangesAsync();
                    break;
                
                case "student":
                    var student = _mapper.Map<Student>(request);
                    student.AppUserId = user.Id;
                    await _studentRepo.AddAsync(student);
                    await _unitOfWork.SaveChangesAsync();
                    break;

                case "parent":
                    var parent = _mapper.Map<Parent>(request);
                    parent.AppUserId = user.Id;
                    await _parentRepo.AddAsync(parent);
                    await _unitOfWork.SaveChangesAsync();
                    break;
            }
            var roles = await _userRepo.GetRolesAsync(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Roles = roles;
            response.Token = GenerateJwtToken(user, roles);

            return ServiceResult<AuthResponse>.Ok(response);
        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null || !await _userRepo.CheckPasswordAsync(user, request.Password))
                return ServiceResult<AuthResponse>.Fail("Invalid credentials");

            var roles = await _userRepo.GetRolesAsync(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Roles = roles;
            response.Token = GenerateJwtToken(user, roles);

            return ServiceResult<AuthResponse>.Ok(response);
        }

        public async Task<ServiceResult<string>> CreateRoleAsync(CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
                return ServiceResult<string>.Fail("Role name cannot be empty");

            var existingRole = await _roleRepo.GetByNameAsync(request.RoleName);
            if (existingRole != null)
                return ServiceResult<string>.Fail("Role already exists");

            var role = new AppRole { Id = Guid.NewGuid().ToString(), Name = request.RoleName };
            var result = await _roleRepo.CreateAsync(role);

            return result.Succeeded
                ? ServiceResult<string>.Ok(request.RoleName, "Role created successfully")
                : ServiceResult<string>.Fail("Failed to create role");
        }

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.UserData,user.UserName)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
