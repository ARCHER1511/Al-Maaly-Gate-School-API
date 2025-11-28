using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IGradeRepository : IGenericRepository<Grade>
    {
        Task<Grade?> GetByIdWithDetailsAsync(string id);
        Task<IEnumerable<Grade>> GetAllWithDetailsAsync();
        Task<Grade?> GetByNameAsync(string gradeName);
        Task<bool> AddClassToGradeAsync(string gradeId, Class classEntity);
        Task<bool> AddSubjectToGradeAsync(string gradeId, Subject subject);
        Task<bool> RemoveClassFromGradeAsync(string classId);
        Task<bool> RemoveSubjectFromGradeAsync(string subjectId);
        Task<bool> MoveClassToAnotherGradeAsync(string classId, string newGradeId);
        Task<bool> MoveSubjectToAnotherGradeAsync(string subjectId, string newGradeId);
        Task<IEnumerable<Class>> GetClassesByGradeIdAsync(string gradeId);
        Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId);
    }
}
