using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Exam> _dbSet;

        public ExamRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Exam>();
        }

        public async Task<Exam?> GetByIdWithQuestionsAsync(int examId)
        {
            return await _dbSet
                .Include(e => e.QuestionExamTeachers)
                    .ThenInclude(qet => qet.Question)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        public async Task<IEnumerable<Exam>> GetByTeacherIdAsync(string teacherId)
        {
            return await _dbSet
                .Include(e => e.QuestionExamTeachers)
                    .ThenInclude(qet => qet.Question)
                .Where(e => e.QuestionExamTeachers.Any(qet => qet.TeacherId == teacherId))
                .ToListAsync();
        }
    }
}
