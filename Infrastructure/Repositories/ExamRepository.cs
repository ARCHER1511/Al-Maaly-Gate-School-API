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

        public async Task<IEnumerable<Exam>> GetByTeacherIdAsync(string teacherId)
        {
            return await _dbSet
                .Include(e => e.Questions)
                .Where(e => e.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdWithQuestionsAsync(string examId)
        {
            return await _dbSet
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }
    }
}
