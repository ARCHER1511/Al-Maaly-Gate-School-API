using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        //Task<Exam?> GetByIdWithQuestionsAsync(int examId);
        //Task<IEnumerable<Exam>> GetByTeacherIdAsync(string teacherId);
    }
}
