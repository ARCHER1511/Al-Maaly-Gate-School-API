using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class CurriculumRepository : GenericRepository<Curriculum>, ICurriculumRepository
    {
        public CurriculumRepository(AlMaalyGateSchoolContext context) : base(context)
        {
        }

        public async Task<Curriculum?> GetWithDetailsAsync(string id)
        {
            return await _context.Curriculums
                .Include(c => c.Grades)
                    .ThenInclude(g => g.Classes)
                .Include(c => c.Grades)
                    .ThenInclude(g => g.Subjects)
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curriculum>> GetAllWithGradesAsync()
        {
            return await _context.Curriculums
                .Include(c => c.Grades)
                .Include(c => c.Students)
                .ToListAsync();
        }

        public async Task<Curriculum?> GetByNameAsync(string name)
        {
            return await _context.Curriculums
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower()!);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Curriculums
                .AnyAsync(c => c.Name.ToLower() == name.ToLower()!);
        }

        // Override to include grades by default
        public override async Task<IEnumerable<Curriculum>> GetAllAsync()
        {
            return await _context.Curriculums
                .Include(c => c.Grades)
                .ToListAsync();
        }

        // Override to include grades by default
        public override async Task<Curriculum?> GetByIdAsync(object id)
        {
            return await _context.Curriculums
                .Include(c => c.Grades)
                .FirstOrDefaultAsync(c => c.Id == (string)id);
        }

        // Optional: Override FirstOrDefaultAsync to include grades by default
        public override async Task<Curriculum?> FirstOrDefaultAsync(
            Expression<Func<Curriculum, bool>> predicate,
            Func<IQueryable<Curriculum>, IIncludableQueryable<Curriculum, object>>? include = null)
        {
            IQueryable<Curriculum> query = _context.Curriculums;

            // Always include Grades by default, then additional includes if provided
            query = query.Include(c => c.Grades);

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}