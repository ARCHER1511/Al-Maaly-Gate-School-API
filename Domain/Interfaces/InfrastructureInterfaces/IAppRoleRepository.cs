using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface IAppRoleRepository
    {
        Task<AppRole?> GetByIdAsync(string roleId);
        Task<AppRole?> GetByNameAsync(string roleName);
        Task<IdentityResult> CreateAsync(AppRole role);
        Task<IdentityResult> UpdateAsync(AppRole role);
        Task<IdentityResult> DeleteAsync(AppRole role);
        Task<IList<AppUser>> GetUsersInRoleAsync(string roleName);
    }
}
