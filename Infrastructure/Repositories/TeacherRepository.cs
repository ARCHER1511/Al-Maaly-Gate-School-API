using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Teacher> _dbSet;

        public TeacherRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Teacher>();
        }

        public async Task<Teacher?> GetByAppUserIdAsync(string appUserId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.AppUserId == appUserId);
        }

        // Add this method for curriculum queries
        public async Task<IEnumerable<Teacher>> GetTeachersWithCurriculaAsync()
        {
            return await _dbSet
                .Include(t => t.SpecializedCurricula)
                .ToListAsync();
        }
        public async Task<Teacher?> GetTeacherWithSubjectsAndClassesByUserIdAsync(string userId)
        {
            return await _dbSet.Where(t => t.AppUserId == userId)
                .Include(t => t.TeacherSubjects)!
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.TeacherClasses)
                    .ThenInclude(t => t.Class)
                    .AsNoTracking()
                .FirstOrDefaultAsync(t => t.AppUserId == userId);
        }

    }
}