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
    }
}
