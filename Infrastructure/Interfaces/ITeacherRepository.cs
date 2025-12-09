using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        Task<Teacher?> GetByAppUserIdAsync(string appUserId);
        Task<Teacher?> GetTeacherWithSubjectsByUserIdAsync(string userId);
    }
}
