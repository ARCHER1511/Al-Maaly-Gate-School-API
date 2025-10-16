using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Authentication;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

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
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
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
            response.Token = JwtExtensions.GenerateJwtToken(user, roles, _config);
            response.UserId = user.Id;

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
            response.Token = JwtExtensions.GenerateJwtToken(user, roles, _config);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = Guid.NewGuid().ToString(),
                AppUserId = user.Id,
                ExpiryDate = DateTime.Now.AddDays(30),
            };
            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            response.RefreshToken = refreshToken.Token;
            response.UserId = user.Id;

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

        public async Task<ServiceResult<string>> AssignRoleAsync(AssignRoleRequest request)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user == null)
                return ServiceResult<string>.Fail($"UserId '{request.UserId}' not found.");

            // Check if role exists
            var roleExists = await _roleRepo.GetByNameAsync(request.RoleName);
            if (roleExists == null)
                return ServiceResult<string>.Fail($"Role '{request.RoleName}' does not exist.");

            // Check if user already has this role
            var userRoles = await _userRepo.GetUserRolesAsync(user);
            if (userRoles.Any(r => r.Equals(request.RoleName, StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<string>.Fail($"User '{user.Email}' already has role '{request.RoleName}'.");

            // Assign role
            var added = await _userRepo.AddToRoleAsync(user, request.RoleName);
            if (!added)
                return ServiceResult<string>.Fail("Failed to assign role.");

            // Common user info (safe fallbacks)
            string fullName = user.FullName ?? "Unknown";
            string email = user.Email ?? "unknown@example.com";
            string contactInfo = user.PhoneNumber ?? "N/A";

            switch (request.RoleName.ToLower())
            {
                case "admin":
                    await _adminRepo.AddAsync(new Admin
                    {
                        AppUserId = user.Id,
                        FullName = fullName,
                        Email = email,
                        ContactInfo = contactInfo,
                        IsApproved = false
                    });
                    break;

                case "teacher":
                    await _teacherRepo.AddAsync(new Teacher
                    {
                        AppUserId = user.Id,
                        FullName = fullName,
                        Email = email,
                        ContactInfo = contactInfo,
                        IsApproved = false
                    });
                    break;

                case "student":
                    await _studentRepo.AddAsync(new Student
                    {
                        AppUserId = user.Id,
                        FullName = fullName,
                        Email = email,
                        ContactInfo = contactInfo,
                        IsApproved = false
                    });
                    break;

                case "parent":
                    await _parentRepo.AddAsync(new Parent
                    {
                        AppUserId = user.Id,
                        FullName = fullName,
                        Email = email,
                        ContactInfo = contactInfo,
                        IsApproved = false
                    });
                    break;

                default:
                    return ServiceResult<string>.Fail($"Unhandled role type '{request.RoleName}'.");
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Role '{request.RoleName}' assigned successfully to '{user.Email}'.");
        }

        public async Task<ServiceResult<string>> UnassignRoleAsync(AssignRoleRequest request)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user == null)
                return ServiceResult<string>.Fail($"UserId '{request.UserId}' not found.");

            var roleExists = await _roleRepo.GetByNameAsync(request.RoleName);
            if (roleExists == null)
                return ServiceResult<string>.Fail($"Role '{request.RoleName}' does not exist.");

            // Check if user actually has this role
            var userRoles = await _userRepo.GetUserRolesAsync(user);
            if (!userRoles.Any(r => r.Equals(request.RoleName, StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<string>.Fail($"User '{user.Email}' does not have role '{request.RoleName}'.");

            // Remove from Identity role
            var removed = await _userRepo.RemoveFromRoleAsync(user, request.RoleName);
            if (!removed)
                return ServiceResult<string>.Fail($"Failed to remove role '{request.RoleName}' from user '{user.Email}'.");

            // Remove from domain entities
            switch (request.RoleName.ToLower())
            {
                case "admin":
                    var admin = await _adminRepo.GetByAppUserIdAsync(user.Id);
                    if (admin != null)
                        _adminRepo.Delete(admin);
                    break;

                case "teacher":
                    var teacher = await _teacherRepo.GetByAppUserIdAsync(user.Id);
                    if (teacher != null)
                        _teacherRepo.Delete(teacher);
                    break;

                case "student":
                    var student = await _studentRepo.GetByAppUserIdAsync(user.Id);
                    if (student != null)
                        _studentRepo.Delete(student);
                    break;

                case "parent":
                    var parent = await _parentRepo.GetByAppUserIdAsync(user.Id);
                    if (parent != null)
                        _parentRepo.Delete(parent);
                    break;

                default:
                    return ServiceResult<string>.Fail($"Unhandled role type '{request.RoleName}'.");
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Role '{request.RoleName}' unassigned successfully from '{user.Email}'.");
        }

        public async Task<ServiceResult<AuthResponse>> RefreshTokenAsync(
            RefreshTokenRequest request
        )
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadJwtToken(request.Token);
            var userId = token.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var savedToken = await _unitOfWork
                .Repository<RefreshToken>()
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken && x.AppUserId == userId);

            if (
                savedToken == null
                || savedToken.IsUsed
                || savedToken.IsRevoked
                || savedToken.ExpiryDate < DateTime.Now
            )
                return ServiceResult<AuthResponse>.Fail("Invalid refresh token");

            savedToken.IsUsed = true;
            await _unitOfWork.SaveChangesAsync();

            var user = await _userRepo.GetByIdAsync(userId);
            var roles = await _userRepo.GetRolesAsync(user!);

            var newJwt = JwtExtensions.GenerateJwtToken(user!, roles, _config);
            var newRefresh = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = Guid.NewGuid().ToString(),
                AppUserId = user!.Id,
                ExpiryDate = DateTime.Now.AddDays(30),
            };
            await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefresh);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponse
            {
                Token = newJwt,
                RefreshToken = newRefresh.Token,
                Roles = roles,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
            };

            return ServiceResult<AuthResponse>.Ok(response);
        }

        public async Task RevokeTokensAsync(string userId)
        {
            var tokens = await _unitOfWork
                .Repository<RefreshToken>()
                .FindAllAsync(t => t.AppUserId == userId && !t.IsRevoked);
            foreach (var t in tokens)
                t.IsRevoked = true;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ServiceResult<AuthResponse>> GetUserProfileAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            var roles = await _userRepo.GetRolesAsync(user!);
            var response = _mapper.Map<AuthResponse>(user);
            response.Roles = roles;
            response.UserId = user!.Id;

            return ServiceResult<AuthResponse>.Ok(response, "Token is valid");
        }

        public async Task<ServiceResult<string>> ChangePasswordAsync(
            string userId,
            ChangePasswordRequest request
        )
        {
            var user = await _userRepo.GetByIdAsync(userId);
            var result = await _userRepo.ChangePasswordAsync(
                user!,
                request.OldPassword,
                request.NewPassword
            );

            return result
                ? ServiceResult<string>.Ok("Password changed successfully")
                : ServiceResult<string>.Fail("Failed to change password");
        }

        public async Task<ServiceResult<string>> ForgotPasswordAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return ServiceResult<string>.Fail("User not found");

            var token = await _userRepo.GeneratePasswordResetTokenAsync(user);
            // TODO: send token via email using IEmailService
            return ServiceResult<string>.Ok(token, "Password reset email sent");
        }

        public async Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
                return ServiceResult<string>.Fail("User not found");

            var result = await _userRepo.ResetPasswordAsync(
                user,
                request.Token,
                request.NewPassword
            );
            return result
                ? ServiceResult<string>.Ok("Password reset successful")
                : ServiceResult<string>.Fail("Invalid token or password");
        }

        public async Task<ServiceResult<string>> DeleteAccountAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<string>.Fail("User not found");

            await _userRepo.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<string>.Ok("Account deleted successfully");
        }

        public async Task<ServiceResult<AuthResponse>> UpdateProfileAsync(
            string userId,
            UpdateProfileRequest request
        )
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<AuthResponse>.Fail("User not found");

            user.FullName = request.FullName;
            user.ContactInfo = request.ContactInfo;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<AuthResponse>(user);
            response.UserId = user.Id;
            return ServiceResult<AuthResponse>.Ok(response, "Profile updated");
        }
    }
}
