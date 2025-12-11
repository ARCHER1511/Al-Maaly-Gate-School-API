using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IGradeRepository : IGenericRepository<Grade>
    {
        Task<Grade?> GetByNameAsync(string gradeName);
        Task<Grade?> GetByNameAndCurriculumAsync(string gradeName, string curriculumId);
        Task<Grade?> GetByIdWithCurriculumAsync(string id);
        Task<Grade?> GetByNameWithCurriculumAsync(string name);
        Task<IEnumerable<Grade>> GetGradesByCurriculumAsync(string curriculumId);
        Task<Grade?> GetByIdWithDetailsAsync(string id);
        Task<IEnumerable<Class>> GetClassesByGradeIdAsync(string gradeId);
        Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId);
        Task<bool> AddSubjectToGradeAsync(string gradeId, Subject subject);
        Task<bool> RemoveClassFromGradeAsync(string classId);
        Task<bool> RemoveSubjectFromGradeAsync(string subjectId);
        Task<bool> MoveClassToAnotherGradeAsync(string classId, string newGradeId);
        Task<bool> MoveSubjectToAnotherGradeAsync(string subjectId, string newGradeId);
    }
}