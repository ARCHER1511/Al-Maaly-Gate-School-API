using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByAppUserIdAsync(string appUserId);
    }
}
