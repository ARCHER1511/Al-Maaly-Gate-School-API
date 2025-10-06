using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class TeacherSubjectExamRepository : ITeacherSubjectExamRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<TeacherSubjectExam> _dbSet;

        public TeacherSubjectExamRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<TeacherSubjectExam>();
        }

        // 🔹 Ordinary methods
        public async Task<TeacherSubjectExam?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TeacherSubjectExam>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(TeacherSubjectExam entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TeacherSubjectExam entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TeacherSubjectExam entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<TeacherSubjectExam?> GetAsync(Expression<Func<TeacherSubjectExam, bool>> predicate,
                                             Func<IQueryable<TeacherSubjectExam>, IIncludableQueryable<TeacherSubjectExam, object>>? include = null)
        {
            IQueryable<TeacherSubjectExam> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TeacherSubjectExam>> GetAllAsync(Expression<Func<TeacherSubjectExam, bool>>? predicate = null,
                                                          Func<IQueryable<TeacherSubjectExam>, IIncludableQueryable<TeacherSubjectExam, object>>? include = null,
                                                          Func<IQueryable<TeacherSubjectExam>, IOrderedQueryable<TeacherSubjectExam>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<TeacherSubjectExam> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<TeacherSubjectExam> Query(Expression<Func<TeacherSubjectExam, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
