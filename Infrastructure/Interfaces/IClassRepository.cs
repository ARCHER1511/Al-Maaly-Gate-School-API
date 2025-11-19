using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<List<Student>> GetStudentsByClassIdAsync(string classId);
        Task<List<Subject>> GetSubjectsByClassIdAsync(string classId);
    }
}
