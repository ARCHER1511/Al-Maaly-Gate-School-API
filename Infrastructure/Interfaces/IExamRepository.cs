using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        Task<IEnumerable<Exam>> GetByTeacherIdAsync(string teacherId);
        Task<Exam?> GetByIdWithQuestionsAsync(string examId);
    }
}
