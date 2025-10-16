using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IParentRepository : IGenericRepository<Parent>
    {
        Task<Parent?> GetByAppUserIdAsync(string appUserId);
    }
}
