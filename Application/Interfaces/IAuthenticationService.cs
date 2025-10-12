using Application.DTOs.AuthDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<string>> CreateRoleAsync(CreateRoleRequest roleName);
        Task<ServiceResult<string>> AssignRoleAsync(AssignRoleRequest request);
        Task<ServiceResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task RevokeTokensAsync(string userId);
        Task<AuthResponse> GetUserProfileAsync(string userId);
        Task<ServiceResult<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<ServiceResult<string>> ForgotPasswordAsync(string email);
        Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ServiceResult<string>> DeleteAccountAsync(string userId);
        Task<ServiceResult<AuthResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    }   
}
