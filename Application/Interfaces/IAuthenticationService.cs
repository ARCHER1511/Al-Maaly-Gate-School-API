using Application.DTOs.AuthDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<string>> CreateRoleAsync(CreateRoleRequest roleName);
    }   
}
