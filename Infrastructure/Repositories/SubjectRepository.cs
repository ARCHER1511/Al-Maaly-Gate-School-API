using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Subject> _dbSet;

        public SubjectRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Subject>();
        }

        // OVERRIDE: Base methods to include Grade by default
        public new async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Grade) // ADDED: Include Grade
                .Include(s => s.TeacherSubjects)
                    .ThenInclude(ts => ts.Teacher)
                .Include(s => s.Exams)
                .ToListAsync();
        }

        public new async Task<Subject?> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(s => s.Grade) // ADDED: Include Grade
                .Include(s => s.TeacherSubjects)
                    .ThenInclude(ts => ts.Teacher)
                .Include(s => s.Exams)
                .Include(s => s.ClassAppointments)
                .Include(s => s.Degrees)
                .FirstOrDefaultAsync(s => s.Id == (string)id);
        }

        public new async Task<IEnumerable<Subject>> FindAllAsync(
            Expression<Func<Subject, bool>>? predicate = null,
            Func<IQueryable<Subject>, IIncludableQueryable<Subject, object>>? include = null,
            Func<IQueryable<Subject>, IOrderedQueryable<Subject>>? orderBy = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<Subject> query = _dbSet;

            // Always include Grade by default
            query = query.Include(s => s.Grade);

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        // ADDITIONAL: Specific methods for Subject
        public async Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId)
        {
            return await _dbSet
                .Where(s => s.GradeId == gradeId)
                .Include(s => s.Grade)
                .Include(s => s.TeacherSubjects)
                    .ThenInclude(ts => ts.Teacher)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsWithTeachersAsync()
        {
            return await _dbSet
                .Include(s => s.Grade)
                .Include(s => s.TeacherSubjects)
                    .ThenInclude(ts => ts.Teacher)
                .ToListAsync();
        }

        public async Task<bool> SubjectExistsAsync(string subjectName, string gradeId)
        {
            return await _dbSet
                .AnyAsync(s => s.SubjectName == subjectName && s.GradeId == gradeId);
        }
    }
}