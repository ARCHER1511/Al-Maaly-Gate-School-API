using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class StudentSubjectExamRepository : IStudentSubjectExamRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<StudentSubjectExam> _dbSet;

        public StudentSubjectExamRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<StudentSubjectExam>();
        }

        // 🔹 Ordinary methods
        public async Task<StudentSubjectExam?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<StudentSubjectExam>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(StudentSubjectExam entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(StudentSubjectExam entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(StudentSubjectExam entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<StudentSubjectExam?> GetAsync(Expression<Func<StudentSubjectExam, bool>> predicate,
                                             Func<IQueryable<StudentSubjectExam>, IIncludableQueryable<StudentSubjectExam, object>>? include = null)
        {
            IQueryable<StudentSubjectExam> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<StudentSubjectExam>> GetAllAsync(Expression<Func<StudentSubjectExam, bool>>? predicate = null,
                                                          Func<IQueryable<StudentSubjectExam>, IIncludableQueryable<StudentSubjectExam, object>>? include = null,
                                                          Func<IQueryable<StudentSubjectExam>, IOrderedQueryable<StudentSubjectExam>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<StudentSubjectExam> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<StudentSubjectExam> Query(Expression<Func<StudentSubjectExam, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
