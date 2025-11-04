using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Question> _dbSet;

        public QuestionRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Question>();
        }

        public async Task<IEnumerable<Question>> GetByTeacherIdAsync(string teacherId)
        {
            return await _dbSet
                .Where(q => q.TeacherId == teacherId)
                .Include(q => q.Choices)
                .Include(q => q.ChoiceAnswer)
                //.Include(q => q.TextAnswer)
                .Include(q => q.Exam)
                .ToListAsync();
        }

        public new async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _dbSet
                .Include(q => q.Choices)
                .Include(q => q.ChoiceAnswer)
                //.Include(q => q.TextAnswer)
                //.Include(q => q.TrueAndFalses)
                .ToListAsync();
        }

        public new async Task<Question?> GetByIdAsync(string id)
        {
            return await _dbSet
                .Include(q => q.Choices)
                .Include(q => q.ChoiceAnswer)
                //.Include(q => q.TextAnswer)
                //.Include(q => q.TrueAndFalses)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
