using Infrastructure.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class AppRoleRepository : IAppRoleRepository
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AppRoleRepository(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<AppRole?> GetByIdAsync(string roleId) =>
            await _roleManager.FindByIdAsync(roleId);

        public async Task<AppRole?> GetByNameAsync(string roleName) =>
            await _roleManager.FindByNameAsync(roleName);

        public async Task<IdentityResult> CreateAsync(AppRole role) =>
            await _roleManager.CreateAsync(role);

        public async Task<IdentityResult> UpdateAsync(AppRole role) =>
            await _roleManager.UpdateAsync(role);

        public async Task<IdentityResult> DeleteAsync(AppRole role) =>
            await _roleManager.DeleteAsync(role);

        public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName) =>
            await _userManager.GetUsersInRoleAsync(roleName);
    }
}
