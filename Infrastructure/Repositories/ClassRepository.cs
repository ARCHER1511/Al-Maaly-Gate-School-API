using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Class> _dbSet;

        public ClassRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Class>();
        }

        public async Task<IEnumerable<Class>> GetAllWithTeachersAsync()
        {
            return await _dbSet
                .Include(c => c.TeacherClasses)
                    .ThenInclude(tc => tc.Teacher)
                .Include(c => c.Grade) // ADDED: Include Grade
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByClassIdAsync(string classId)
        {
            return await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsByClassIdAsync(string classId)
        {
            // First get the class to find its grade
            var classEntity = await _context.Classes
                .Include(c => c.Grade)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classEntity == null || classEntity.Grade == null)
                return new List<Subject>();

            // Now get all subjects for that grade (since subjects are now at grade level)
            return await _context.Subjects
                .Where(s => s.GradeId == classEntity.GradeId)
                .Include(s => s.Grade) // ADDED: Include Grade for mapping
                .ToListAsync();
        }

        // OVERRIDE: Base methods to include Grade by default
        public new async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Grade) // ADDED: Include Grade
                .Include(c => c.TeacherClasses)
                    .ThenInclude(tc => tc.Teacher)
                .Include(c => c.Students)
                .ToListAsync();
        }

        public new async Task<Class?> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(c => c.Grade) // ADDED: Include Grade
                .Include(c => c.TeacherClasses)
                    .ThenInclude(tc => tc.Teacher)
                .Include(c => c.Students)
                .Include(c => c.ClassAssets)
                .Include(c => c.ClassAppointments)
                .FirstOrDefaultAsync(c => c.Id == (string)id);
        }

        public new async Task<IEnumerable<Class>> FindAllAsync(
            Expression<Func<Class, bool>>? predicate = null,
            Func<IQueryable<Class>, IIncludableQueryable<Class, object>>? include = null,
            Func<IQueryable<Class>, IOrderedQueryable<Class>>? orderBy = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<Class> query = _dbSet;

            // Always include Grade by default
            query = query.Include(c => c.Grade);

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }
    }
}