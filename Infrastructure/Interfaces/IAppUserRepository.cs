using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Interfaces
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetByIdAsync(string userId);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(AppUser user);
        Task<bool> AddToRoleAsync(AppUser user, string role);
        Task<bool> RemoveFromRoleAsync(AppUser user, string role);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task<IdentityResult> CreateAsync(AppUser user, string password);
        Task<IdentityResult> UpdateAsync(AppUser user);
        Task<IdentityResult> DeleteAsync(AppUser user);
        Task<bool> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(AppUser user);
        Task<bool> ResetPasswordAsync(AppUser user, string token, string newPassword);
        Task<IList<string>> GetUserRolesAsync(AppUser user);
        Task<List<AppUser>> GetAllUsersAsync();
    }
}
