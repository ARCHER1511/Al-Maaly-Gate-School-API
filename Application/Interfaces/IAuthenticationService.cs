using Application.DTOs.AdminDTOs;
using Application.DTOs.AuthDTOs;
using Application.DTOs.ParentDTOs;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<ParentRegistrationResponse>> RegisterParentAsync(
            ParentRegisterRequest request
        );
        Task<ServiceResult<string>> CreateTeacherAsync(CreateTeacherRequest request);
        Task<ServiceResult<string>> CreateStudentAsync(CreateStudentRequest request);
        Task<ServiceResult<string>> CreateParentAsync(CreateParentRequest request);
        Task<ServiceResult<List<object>>> BulkCreateUsersAsync(BulkCreateUsersRequest request);
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<string>> CreateRoleAsync(CreateRoleRequest roleName);
        Task<ServiceResult<string>> AssignRoleAsync(AssignRoleRequest request);
        Task<ServiceResult<string>> UnassignRoleAsync(AssignRoleRequest request);
        Task<ServiceResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task RevokeTokensAsync(string userId);
        Task<ServiceResult<AuthResponse>> GetUserProfileAsync(string userId);
        Task<ServiceResult<string>> ChangePasswordAsync(
            string userId,
            ChangePasswordRequest request
        );
        Task<ServiceResult<string>> ForgotPasswordAsync(string email);
        Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ServiceResult<string>> DeleteAccountAsync(string userId);
        Task<ServiceResult<AuthResponse>> UpdateProfileAsync(
            string userId,
            UpdateProfileRequest request
        );
        Task<ServiceResult<AuthResponse>> UploadProfilePhotoAsync(string userId, IFormFile file);
        Task<ServiceResult<List<AppUser>>> GetPendingRoleTeacherAsync();
        Task<ServiceResult<List<AppUser>>> GetPendingRoleStudentAsync();
        Task<ServiceResult<List<AppUser>>> GetPendingRoleParentAsync();
        Task<ServiceResult<string>> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<ServiceResult<string>> ResendConfirmationAsync(ResendConfirmationRequest request);
    }
}
