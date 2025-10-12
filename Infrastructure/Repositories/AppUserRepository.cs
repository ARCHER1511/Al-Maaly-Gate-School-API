using Infrastructure.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public AppUserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser?> GetByIdAsync(string userId) =>
            await _userManager.FindByIdAsync(userId);

        public async Task<AppUser?> GetByEmailAsync(string email) =>
            await _userManager.FindByEmailAsync(email);

        public async Task<IList<string>> GetRolesAsync(AppUser user) =>
            await _userManager.GetRolesAsync(user);

        public async Task<bool> AddToRoleAsync(AppUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveFromRoleAsync(AppUser user, string role)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> CheckPasswordAsync(AppUser user, string password) =>
            await _userManager.CheckPasswordAsync(user, password);

        public async Task<IdentityResult> CreateAsync(AppUser user, string password) =>
            await _userManager.CreateAsync(user, password);

        public async Task<IdentityResult> UpdateAsync(AppUser user) =>
            await _userManager.UpdateAsync(user);

        public async Task<IdentityResult> DeleteAsync(AppUser user) =>
            await _userManager.DeleteAsync(user);

        public async Task<bool> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(AppUser user)
        {
            // Generate a secure token (used in reset password link)
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            // Reset password using the provided token
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
    }
}
