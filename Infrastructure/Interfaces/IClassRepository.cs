using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<IEnumerable<Class>> GetAllWithTeachersAsync();
        Task<List<Student>> GetStudentsByClassIdAsync(string classId);
        Task<List<Subject>> GetSubjectsByClassIdAsync(string classId);
    }
}
