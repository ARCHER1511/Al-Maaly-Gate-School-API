using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Authentication;
using Application.DTOs.AuthDTOs;
using Application.DTOs.FileRequestDTOs;
using Application.DTOs.ParentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly IFileService _fileService;
        private readonly IFileRecordRepository _fileRecordRepository;

        public AuthenticationService(
            IAppUserRepository userRepo,
            IAppRoleRepository roleRepo,
            IAdminRepository adminRepo,
            ITeacherRepository teacherRepo,
            IStudentRepository studentRepo,
            IParentRepository parentRepo,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMapper mapper,
            IFileService fileService,
            IFileRecordRepository fileRecordRepository
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
            _fileService = fileService;
            _fileRecordRepository = fileRecordRepository;
        }

        private async Task CleanupUploadedFiles(List<string> filePaths)
        {
            foreach (var path in filePaths)
            {
                try
                {
                    await _fileService.DeleteFileAsync(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine( $"Failed to cleanup file during rollback: {path} information: {ex}");
                }
            }
        }

        private async Task LinkFilesToParent(string parentId, List<string> filePaths, string identityDocumentName)
        {
            bool identityDocumentProcessed = false;

            foreach (var filePath in filePaths)
            {
                // Get the FileRecord
                var fileRecordResult = await _fileService.GetFileByPathAsync(filePath);
                if (!fileRecordResult.Success || fileRecordResult.Data == null)
                    continue;

                var fileRecord = fileRecordResult.Data;

                if (!identityDocumentProcessed ||
                    fileRecord.FileName.Contains(Path.GetFileNameWithoutExtension(identityDocumentName)))
                {
                    fileRecord.FileType = "identity";
                    identityDocumentProcessed = true;
                }
                else
                {
                    fileRecord.FileType = "additional";
                }

                fileRecord.Id = parentId;

                await _fileRecordRepository.UpdateAsync(fileRecord);
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<List<DocumentInfo>> GetUploadedDocumentInfos(List<string> filePaths)
        {
            var documentInfos = new List<DocumentInfo>();

            foreach (var path in filePaths)
            {
                var fileResult = await _fileService.GetFileByPathAsync(path);
                if (fileResult.Success && fileResult.Data != null)
                {
                    documentInfos.Add(new DocumentInfo
                    {
                        Id = fileResult.Data.Id,
                        Path = fileResult.Data.RelativePath,
                        Type = fileResult.Data.FileType ?? "unknown",
                        OriginalFileName = fileResult.Data.FileName,
                        FileSize = fileResult.Data.FileSize,
                        UploadedAt = fileResult.Data.UploadedAt
                    });
                }
            }

            return documentInfos;
        }

        public async Task<ServiceResult<ParentRegistrationResponse>> RegisterParentWithDocumentsAsync(ParentRegisterRequest request)
        {
            List<string> uploadedFilePaths = new List<string>();

            try
            {
                if (request.IdentityDocument == null || request.IdentityDocument.Length == 0)
                    return ServiceResult<ParentRegistrationResponse>.Fail("Identity document is required");

                var allFiles = new List<IFormFile> { request.IdentityDocument };
                if (request.AdditionalDocuments != null && request.AdditionalDocuments.Any())
                    allFiles.AddRange(request.AdditionalDocuments);

                var uploadResult = await _fileService.UploadFilesAsync(allFiles, "parents");
                if (!uploadResult.Success)
                    return ServiceResult<ParentRegistrationResponse>.Fail(uploadResult.Message);

                uploadedFilePaths = uploadResult.Data!;

                var baseRequest = _mapper.Map<RegisterRequest>(request);
                baseRequest.Role = "parent"; // Force role to parent

                var authResult = await RegisterAsync(baseRequest);
                if (!authResult.Success)
                {
                    await CleanupUploadedFiles(uploadedFilePaths);
                    return ServiceResult<ParentRegistrationResponse>.Fail(authResult.Message);
                }

                var existingParent = await _parentRepo.GetByAppUserIdAsync(authResult.Data!.UserId);

                Parent newParent;

                if (existingParent != null)
                {
                    existingParent.Relation = request.Relation;
                    existingParent.ContactInfo = request.ContactInfo ?? "Not Provided";
                    newParent = existingParent;
                    _parentRepo.Update(newParent);
                }
                else
                {
                    newParent = _mapper.Map<Parent>(request);
                    newParent.AppUserId = authResult.Data!.UserId;
                    await _parentRepo.AddAsync(newParent);
                }

                await _unitOfWork.SaveChangesAsync();

                var documentInfos = await GetUploadedDocumentInfos(uploadedFilePaths);


                var parentProfile = new ParentProfileDto
                {
                    Id = newParent.Id,
                    RelationshipToStudent = newParent.Relation ?? string.Empty,
                    DocumentCount = documentInfos.Count
                };

                var response = new ParentRegistrationResponse
                {
                    UserId = authResult.Data!.UserId,
                    Email = authResult.Data.Email,
                    FullName = authResult.Data.FullName,
                    Token = authResult.Data.Token,
                    Roles = authResult.Data.Roles,
                    ProfileImageUrl = authResult.Data.ProfileImageUrl,
                    RoleEntityIds = authResult.Data.RoleEntityIds,

                    UploadedDocuments = documentInfos,
                    ParentProfile = parentProfile
                };

                return ServiceResult<ParentRegistrationResponse>.Ok(response, "Parent registered successfully with documents");
            }
            catch (Exception ex)
            {
                // Cleanup on any exception
                if (uploadedFilePaths.Any())
                    await CleanupUploadedFiles(uploadedFilePaths);
                return ServiceResult<ParentRegistrationResponse>.Fail($"Registration failed: {ex.Message}");
            }
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            // 1️⃣ Password confirmation
            if (request.Password != request.ConfirmPassword)
                return ServiceResult<AuthResponse>.Fail("Passwords do not match.");

            // 2️⃣ Check for existing user
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return ServiceResult<AuthResponse>.Fail("Email already registered.");

            // 3️⃣ Validate role
            if (string.IsNullOrWhiteSpace(request.Role))
                return ServiceResult<AuthResponse>.Fail("Role is required.");

            string normalizedRole = request.Role.Trim().ToLowerInvariant();
            var validRoles = new[] { "admin", "teacher", "student", "parent" };
            if (!validRoles.Contains(normalizedRole))
                return ServiceResult<AuthResponse>.Fail($"Invalid role '{request.Role}'.");

            // 4️⃣ Create AppUser
            var user = _mapper.Map<AppUser>(request);
            if (string.IsNullOrEmpty(user.Id))
                user.Id = Guid.NewGuid().ToString();

            var createResult = await _userRepo.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return ServiceResult<AuthResponse>.Fail($"User creation failed: {errors}");
            }

            // 5️⃣ Assign role
            await _userRepo.AddToRoleAsync(user, normalizedRole);

            // 6️⃣ Create role-specific entity
            string roleEntityId = string.Empty;
            Teacher? teacherEntity = null;

            switch (normalizedRole)
            {
                case "admin":
                    var admin = _mapper.Map<Admin>(request);
                    admin.AppUserId = user.Id;
                    await _adminRepo.AddAsync(admin);
                    roleEntityId = admin.Id;
                    break;

                case "teacher":
                    var teacher = _mapper.Map<Teacher>(request);
                    teacher.AppUserId = user.Id;
                    await _teacherRepo.AddAsync(teacher);
                    roleEntityId = teacher.Id;
                    teacherEntity = teacher;
                    break;

                case "student":
                    var student = _mapper.Map<Student>(request);
                    student.AppUserId = user.Id;
                    await _studentRepo.AddAsync(student);
                    roleEntityId = student.Id;
                    break;

                case "parent":
                    var parent = _mapper.Map<Parent>(request);
                    parent.AppUserId = user.Id;
                    await _parentRepo.AddAsync(parent);
                    roleEntityId = parent.Id;
                    break;
            }

            // 7️⃣ Commit all changes once
            await _unitOfWork.SaveChangesAsync();

            // 8️⃣ Prepare response data
            var roles = await _userRepo.GetRolesAsync(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.UserId = user.Id;
            response.Roles = roles;
            response.Token = JwtExtensions.GenerateJwtToken(user,teacherEntity,roles, _config);
            response.ProfileImageUrl = user.ProfileImagePath ?? "/uploads/users/default.png";
            response.RoleEntityIds = new Dictionary<string, string?>
            {
                { $"{char.ToUpper(normalizedRole[0]) + normalizedRole.Substring(1)}Id", roleEntityId }
            }!;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            if (normalizedRole == "teacher")
            {
                teacherEntity = await _teacherRepo.GetTeacherWithSubjectsAndClassesByUserIdAsync(user.Id);
            }
            return ServiceResult<AuthResponse>.Ok(response, "Registration successful.");
        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null || !await _userRepo.CheckPasswordAsync(user, request.Password))
                return ServiceResult<AuthResponse>.Fail("Invalid credentials");

            var roles = await _userRepo.GetRolesAsync(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Roles = roles;
            Teacher? teacherEntity = null;
            if (roles.Contains("Teacher"))
            {
                teacherEntity = await _teacherRepo.GetTeacherWithSubjectsAndClassesByUserIdAsync(user.Id);
            }
            response.UserId = user.Id;
            response.ProfileImageUrl = user.ProfileImagePath;
            response.RoleEntityIds = await BuildRoleEntityIdMap(user);
            response.Token = JwtExtensions.GenerateJwtToken(user,teacherEntity, roles, _config);

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
                return ServiceResult<string>.Fail(
                    $"User '{user.Email}' already has role '{request.RoleName}'."
                );

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
                    await _adminRepo.AddAsync(
                        new Admin
                        {
                            AppUserId = user.Id,
                            FullName = fullName,
                            Email = email,
                            ContactInfo = contactInfo,
                            AccountStatus = AccountStatus.Active,
                        }
                    );
                    break;

                case "teacher":
                    await _teacherRepo.AddAsync(
                        new Teacher
                        {
                            AppUserId = user.Id,
                            FullName = fullName,
                            Email = email,
                            ContactInfo = contactInfo,
                            AccountStatus = AccountStatus.Pending,
                        }
                    );
                    break;

                case "student":
                    await _studentRepo.AddAsync(
                        new Student
                        {
                            AppUserId = user.Id,
                            FullName = fullName,
                            Email = email,
                            ContactInfo = contactInfo,
                            AccountStatus = AccountStatus.Pending,
                        }
                    );
                    break;

                case "parent":
                    await _parentRepo.AddAsync(
                        new Parent
                        {
                            AppUserId = user.Id,
                            FullName = fullName,
                            Email = email,
                            ContactInfo = contactInfo,
                            AccountStatus = AccountStatus.Pending,
                        }
                    );
                    break;

                default:
                    return ServiceResult<string>.Fail($"Unhandled role type '{request.RoleName}'.");
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<string>.Ok(
                $"Role '{request.RoleName}' assigned successfully to '{user.Email}'."
            );
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
                return ServiceResult<string>.Fail(
                    $"User '{user.Email}' does not have role '{request.RoleName}'."
                );

            // Remove from Identity role
            var removed = await _userRepo.RemoveFromRoleAsync(user, request.RoleName);
            if (!removed)
                return ServiceResult<string>.Fail(
                    $"Failed to remove role '{request.RoleName}' from user '{user.Email}'."
                );

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

            return ServiceResult<string>.Ok(
                $"Role '{request.RoleName}' unassigned successfully from '{user.Email}'."
            );
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
            Teacher? teacherEntity = null;
            if (roles.Contains("Teacher"))
            {
                teacherEntity = await _teacherRepo.GetTeacherWithSubjectsAndClassesByUserIdAsync(userId);
            }

            var newJwt = JwtExtensions.GenerateJwtToken(user!, teacherEntity, roles, _config);
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
            response.ProfileImageUrl = user.ProfileImagePath;
            response.RoleEntityIds = await BuildRoleEntityIdMap(user);

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
            response.ProfileImageUrl = user.ProfileImagePath;
            response.RoleEntityIds = await BuildRoleEntityIdMap(user);
            return ServiceResult<AuthResponse>.Ok(response, "Profile updated");
        }

        public async Task<ServiceResult<AuthResponse>> UploadProfilePhotoAsync(
            string userId,
            IFormFile file
        )
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<AuthResponse>.Fail("User not found");

            var upload = await _fileService.UploadFileAsync(file, "AfterAuthentication"); // controllerName "users"
            if (!upload.Success)
                return ServiceResult<AuthResponse>.Fail(upload.Message);

            // optional: remove old physical file via fileService.DeleteFileAsync(user.ProfileImagePath) if exists
            user.ProfileImagePath = upload.Data!; // store relative path

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var roles = await _userRepo.GetRolesAsync(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Roles = roles;
            response.ProfileImageUrl = user.ProfileImagePath;
            response.RoleEntityIds = await BuildRoleEntityIdMap(user);

            return ServiceResult<AuthResponse>.Ok(response, "Profile photo uploaded");
        }

        private async Task<Dictionary<string, string>> BuildRoleEntityIdMap(AppUser user)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var roles = await _userRepo.GetRolesAsync(user);

            foreach (var role in roles)
            {
                switch (role.ToLower())
                {
                    case "admin":
                    {
                        var admin = await _adminRepo.GetByAppUserIdAsync(user.Id);
                        if (admin != null)
                            map["adminId"] = admin.Id;
                        break;
                    }
                    case "teacher":
                    {
                        var t = await _teacherRepo.GetByAppUserIdAsync(user.Id);
                        if (t != null)
                            map["teacherId"] = t.Id;
                        break;
                    }
                    case "student":
                    {
                        var s = await _studentRepo.GetByAppUserIdAsync(user.Id);
                        if (s != null)
                            map["studentId"] = s.Id;
                        break;
                    }
                    case "parent":
                    {
                        var p = await _parentRepo.GetByAppUserIdAsync(user.Id);
                        if (p != null)
                            map["parentId"] = p.Id;
                        break;
                    }
                }
            }

            return map;
        }
    }
}
