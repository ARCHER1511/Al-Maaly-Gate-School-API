using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Task<Admin?> GetByAppUserIdAsync(string appUserId);
    }
}
