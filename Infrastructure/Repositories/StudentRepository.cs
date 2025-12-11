using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentRepository : GenericRepository<Student>,IStudentRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Student> _dbSet;

        public StudentRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Student>();
        }
        public async Task<Student?> GetByAppUserIdAsync(string appUserId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.AppUserId == appUserId);
        }

        public async Task<Student?> GetByIdWithDetailsAsync(object id)
        {
            return await _context.Students
                .Include(s => s.Class)
                    .ThenInclude(c => c!.Grade)
                        .ThenInclude(g => g!.Curriculum)
                .Include(s => s.Curriculum)
                .Include(s => s.Parents)
                    .ThenInclude(ps => ps.Parent)
                .FirstOrDefaultAsync(s => s.Id == (string)id);
        }

        public async Task<IEnumerable<Student>> GetAllWithDetailsAsync()
        {
            return await _context.Students
                .Include(s => s.Class)
                    .ThenInclude(c => c!.Grade)
                        .ThenInclude(g => g!.Curriculum)
                .Include(s => s.Curriculum)
                .Include(s => s.Parents)
                    .ThenInclude(ps => ps.Parent)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsByCurriculumAsync(string curriculumId)
        {
            return await _context.Students
                .Include(s => s.Class)
                    .ThenInclude(c => c!.Grade)
                .Include(s => s.Curriculum)
                .Where(s => s.CurriculumId == curriculumId)
                .ToListAsync();
        }

        public async Task<int> GetStudentCountByCurriculumAsync(string curriculumId)
        {
            return await _context.Students
                .CountAsync(s => s.CurriculumId == curriculumId);
        }
    }
}
