using Domain.Entities;

namespace Infrastructure.Interfaces
{
   
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByAppUserIdAsync(string appUserId);
        Task<Student?> GetByIdWithDetailsAsync(object id);
        Task<IEnumerable<Student>> GetAllWithDetailsAsync();
        Task<IEnumerable<Student>> GetStudentsByCurriculumAsync(string curriculumId);
        Task<int> GetStudentCountByCurriculumAsync(string curriculumId);
    }
}
