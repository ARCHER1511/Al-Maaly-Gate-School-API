using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId);
        Task<IEnumerable<Subject>> GetSubjectsWithTeachersAsync();
        Task<bool> SubjectExistsAsync(string subjectName, string gradeId);
    }
}
