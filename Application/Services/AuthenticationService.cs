using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Application.Authentication;
using Application.DTOs.AuthDTOs;
using Application.DTOs.FileRequestDTOs;
using Application.DTOs.ParentDTOs;
using Application.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

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
        private readonly IEmailService _emailService;

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
            IFileRecordRepository fileRecordRepository,
            IEmailService emailService
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
            _emailService = emailService;
        }

        private async Task CleanupUploadedFiles(List<string> filePaths, string userId)
        {
            foreach (var path in filePaths)
            {
                try
                {
                    await _fileService.DeleteFileAsync(path, userId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Failed to cleanup file during rollback: {path} information: {ex}"
                    );
                }
            }
        }

        private int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            int age = today.Year - birthDate.Year;

            // If birthday hasn't occurred yet this year, subtract 1
            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        private async Task LinkFilesToParent(
            string parentId,
            List<string> filePaths,
            string identityDocumentName,
            string userId
        )
        {
            bool identityDocumentProcessed = false;

            foreach (var filePath in filePaths)
            {
                // Get the FileRecord
                var fileRecordResult = await _fileService.GetFileByPathAsync(filePath, userId);
                if (!fileRecordResult.Success || fileRecordResult.Data == null)
                    continue;

                var fileRecord = fileRecordResult.Data;

                if (
                    !identityDocumentProcessed
                    || fileRecord.FileName.Contains(
                        Path.GetFileNameWithoutExtension(identityDocumentName)
                    )
                )
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

        private async Task<List<DocumentInfo>> GetUploadedDocumentInfos(
            List<string> filePaths,
            string userId
        )
        {
            var documentInfos = new List<DocumentInfo>();

            foreach (var path in filePaths)
            {
                var fileResult = await _fileService.GetFileByPathAsync(path, userId);
                if (fileResult.Success && fileResult.Data != null)
                {
                    documentInfos.Add(
                        new DocumentInfo
                        {
                            Id = fileResult.Data.Id,
                            Path = fileResult.Data.RelativePath,
                            Type = fileResult.Data.FileType ?? "unknown",
                            OriginalFileName = fileResult.Data.FileName,
                            FileSize = fileResult.Data.FileSize,
                            UploadedAt = fileResult.Data.UploadedAt,
                        }
                    );
                }
            }
            return documentInfos;
        }

        public async Task<ServiceResult<ParentRegistrationResponse>> RegisterParentAsync(
            ParentRegisterRequest request
        )
        {
            try
            {
                // Map to base RegisterRequest and force role
                var baseRequest = _mapper.Map<RegisterRequest>(request);
                baseRequest.Role = "parent";

                // Register user
                var authResult = await RegisterAsync(baseRequest);
                if (!authResult.Success)
                    return ServiceResult<ParentRegistrationResponse>.Fail(authResult.Message);

                // Build response
                var response = new ParentRegistrationResponse
                {
                    UserId = authResult.Data!.UserId,
                    Email = authResult.Data.Email,
                    RequiresConfirmation = true,
                    //FullName = authResult.Data.FullName,
                    //Token = authResult.Data.Token,
                    //Roles = authResult.Data.Roles,
                    //ProfileImageUrl = authResult.Data.ProfileImageUrl,
                    //RoleEntityIds = authResult.Data.RoleEntityIds,

                    //ParentProfile = new ParentProfileDto
                    //{
                    //    Id = parent.Id,
                    //    RelationshipToStudent = parent.Relation ?? string.Empty,
                    //    DocumentCount = 0, // no documents yet
                    //},
                };

                return ServiceResult<ParentRegistrationResponse>.Ok(
                    response,
                    "Parent registered successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<ParentRegistrationResponse>.Fail(
                    $"Registration failed: {ex.Message}"
                );
            }
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return ServiceResult<AuthResponse>.Fail("Passwords do not match.");

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return ServiceResult<AuthResponse>.Fail("Email already registered.");

            if (string.IsNullOrWhiteSpace(request.Role))
                return ServiceResult<AuthResponse>.Fail("Role is required.");

            var role = request.Role.Trim().ToLowerInvariant();
            var validRoles = new[] { "admin", "teacher", "student", "parent" };
            if (!validRoles.Contains(role))
                return ServiceResult<AuthResponse>.Fail("Invalid role.");

            int age = CalculateAge(request.BirthDay);

            var user = _mapper.Map<AppUser>(request);
            user.Id = Guid.NewGuid().ToString();
            user.EmailConfirmed = false;
            user.PendingRole = role; //
            user.ConfirmationNumber = GenerateConfirmationNumber();
            user.EmailConfirmationToken = GenerateEmailToken();
            user.ConfirmationTokenExpiry = DateTime.Now.AddHours(24);
            user.Age = age;

            var result = await _userRepo.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return ServiceResult<AuthResponse>.Fail(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

            await SendConfirmationEmail(user);

            return ServiceResult<AuthResponse>.Ok(
                new AuthResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    RequiresConfirmation = true,
                },
                "Registration successful. Please confirm your email."
            );
        }

        public async Task<ServiceResult<string>> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            AppUser? user = null;

            if (!string.IsNullOrEmpty(request.Token) && !string.IsNullOrEmpty(request.UserId))
                user = await _userRepo.GetByIdAsync(request.UserId);
            else if (
                !string.IsNullOrEmpty(request.ConfirmationNumber)
                && !string.IsNullOrEmpty(request.Email)
            )
                user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null)
                return ServiceResult<string>.Fail("User not found.");

            if (user.EmailConfirmed)
                return ServiceResult<string>.Ok("Email already confirmed.");

            if (user.ConfirmationTokenExpiry < DateTime.Now)
                return ServiceResult<string>.Fail("Confirmation expired.");

            if (
                user.EmailConfirmationToken != request.Token
                && user.ConfirmationNumber != request.ConfirmationNumber
            )
                return ServiceResult<string>.Fail("Invalid confirmation data.");

            // ✅ CONFIRM EMAIL
            user.EmailConfirmed = true;
            user.ConfirmationNumber = null;
            user.EmailConfirmationToken = null;
            user.ConfirmationTokenExpiry = null;

            // ✅ ASSIGN ROLE
            var role = user.PendingRole!;
            await _userRepo.AddToRoleAsync(user, role);

            // ✅ CREATE ROLE ENTITY
            await CreateRoleEntities(user, role);

            user.PendingRole = null;

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<string>.Ok("Account activated successfully.");
        }

        private async Task CreateRoleEntities(AppUser user, string role)
        {
            switch (role)
            {
                case "admin":
                    await _adminRepo.AddAsync(
                        new Admin
                        {
                            AppUserId = user.Id,
                            AccountStatus = AccountStatus.Pending,
                            ContactInfo = user.ContactInfo,
                            Email = user.Email!,
                            FullName = user.FullName,
                        }
                    );
                    break;

                case "teacher":
                    await _teacherRepo.AddAsync(
                        new Teacher
                        {
                            AppUserId = user.Id,
                            AccountStatus = AccountStatus.Pending,
                            ContactInfo = user.ContactInfo,
                            Email = user.Email!,
                            FullName = user.FullName,
                        }
                    );
                    break;

                case "student":
                    await _studentRepo.AddAsync(
                        new Student
                        {
                            AppUserId = user.Id,
                            AccountStatus = AccountStatus.Pending,
                            ContactInfo = user.ContactInfo,
                            Email = user.Email!,
                            FullName = user.FullName,
                        }
                    );
                    break;

                case "parent":
                    await _parentRepo.AddAsync(
                        new Parent
                        {
                            AppUserId = user.Id,
                            AccountStatus = AccountStatus.Pending,
                            ContactInfo = user.ContactInfo,
                            Email = user.Email!,
                            FullName = user.FullName,
                        }
                    );
                    break;
            }
        }

        public async Task<ServiceResult<string>> ResendConfirmationAsync(
            ResendConfirmationRequest request
        )
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
                return ServiceResult<string>.Fail("User not found.");

            if (user.EmailConfirmed)
                return ServiceResult<string>.Fail("Email already confirmed.");

            user.ConfirmationNumber = GenerateConfirmationNumber();
            user.EmailConfirmationToken = GenerateEmailToken();
            user.ConfirmationTokenExpiry = DateTime.Now.AddHours(24);

            await _userRepo.UpdateAsync(user);
            await SendConfirmationEmail(user);

            return ServiceResult<string>.Ok("Confirmation email resent.");
        }

        private async Task SendConfirmationEmail(AppUser user)
        {
            var token = UrlEncoder.Default.Encode(user.EmailConfirmationToken!);
            var link =
                $"{_config["App:BaseUrl"]}/api/authentication/confirm-email?token={token}&userId={user.Id}";

            var body =
                $@"
                    <div style='font-family:Arial'>
                        <h2>Email Confirmation</h2>
                        <p>Your confirmation code:</p>
                        <h3>{user.ConfirmationNumber}</h3>
                        <p>
                        <a href='{link}' style='padding:10px 15px;background:#0d6efd;color:#fff;text-decoration:none;border-radius:5px'>
                            Confirm Email
                        </a>
                        </p>
                        <p style='color:gray'>Expires in 24 hours</p>
                    </div>";

            await _emailService.SendAsync(user.Email!, "Confirm Email", body, true);
        }

        private string GenerateConfirmationNumber() =>
            Random.Shared.Next(100000, 999999).ToString();

        private string GenerateEmailToken() =>
            Convert
                .ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null || !await _userRepo.CheckPasswordAsync(user, request.Password))
                return ServiceResult<AuthResponse>.Fail("Invalid credentials");

            if (!user.EmailConfirmed)
                return ServiceResult<AuthResponse>.Fail("Please confirm your email first.");

            var roles = await _userRepo.GetRolesAsync(user);

            var response = _mapper.Map<AuthResponse>(user);
            response.Roles = roles;
            Teacher? teacherEntity = null;
            if (roles.Contains("Teacher"))
            {
                teacherEntity = await _teacherRepo.GetTeacherWithSubjectsAndClassesByUserIdAsync(
                    user.Id
                );
            }
            response.UserId = user.Id;
            response.ProfileImageUrl = user.ProfileImagePath;
            response.RoleEntityIds = await BuildRoleEntityIdMap(user);
            response.Token = JwtExtensions.GenerateJwtToken(user, teacherEntity, roles, _config);

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
                teacherEntity = await _teacherRepo.GetTeacherWithSubjectsAndClassesByUserIdAsync(
                    userId
                );
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
                return ServiceResult<string>.Ok("If the email exists, a reset link has been sent");

            var token = await _userRepo.GeneratePasswordResetTokenAsync(user);
            //send token via email using IEmailService
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(email);
            //it can be set in appsetting
            var resetUrl =
                $"{_config["Frontend:ResetPasswordUrl"]}"
                + $"?email={encodedEmail}&token={encodedToken}";

            var body =
                $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Password Reset</title>
</head>
<body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Arial,Helvetica,sans-serif;'>

    <table width='100%' cellpadding='0' cellspacing='0'>
        <tr>
            <td align='center' style='padding:30px 15px;'>
                <table width='600' cellpadding='0' cellspacing='0'
                       style='background:#ffffff;border-radius:8px;box-shadow:0 4px 10px rgba(0,0,0,0.08);'>

                    <tr>
                        <td style='padding:30px;'>

                            <h2 style='color:#333;margin-top:0;'>Reset your password</h2>

                            <p style='color:#555;font-size:14px;line-height:1.6;'>
                                Hello <strong>{user.FullName}</strong>,
                            </p>

                            <p style='color:#555;font-size:14px;line-height:1.6;'>
                                You recently requested to reset your password.
                                Click the button below to proceed.
                            </p>

                            <div style='text-align:center;margin:30px 0;'>
                                <a href='{resetUrl}'
                                   style='background-color:#2563eb;
                                          color:#ffffff;
                                          padding:12px 24px;
                                          text-decoration:none;
                                          border-radius:6px;
                                          font-size:15px;
                                          display:inline-block;'>
                                    Reset Password
                                </a>
                            </div>

                            <p style='color:#555;font-size:13px;line-height:1.6;'>
                                This password reset link will expire shortly.
                            </p>

                            <p style='color:#888;font-size:12px;line-height:1.6;'>
                                If you did not request this password reset, you can safely ignore this email.
                                Your account will remain secure.
                            </p>

                            <hr style='border:none;border-top:1px solid #eee;margin:25px 0;' />

                            <p style='color:#aaa;font-size:11px;text-align:center;'>
                                © {DateTime.UtcNow.Year} Your Company Name
                            </p>

                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>

</body>
</html>";

            await _emailService.SendAsync(user.Email!, "Reset your password", body, isHtml: true);

            return ServiceResult<string>.Ok("If the email exists, a reset link has been sent");
        }

        public async Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
                return ServiceResult<string>.Fail("Invalid reset request");

            var success = await _userRepo.ResetPasswordAsync(
                user,
                request.Token,
                request.NewPassword
            );
            return success
                ? ServiceResult<string>.Ok("Password reset successful")
                : ServiceResult<string>.Fail("Invalid or expired token");
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

            var upload = await _fileService.UploadFileAsync(file, "AfterAuthentication", userId); // controllerName "users"
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
