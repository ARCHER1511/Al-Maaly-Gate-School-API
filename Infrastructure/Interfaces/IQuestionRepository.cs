using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<IEnumerable<Question>> GetByTeacherIdAsync(string teacherId);
    }
}
