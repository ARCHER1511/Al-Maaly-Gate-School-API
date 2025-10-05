using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Exam> _dbSet;

        public ExamRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Exam>();
        }

        // 🔹 Ordinary methods
        public async Task<Exam?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Exam entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Exam entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Exam entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Exam?> GetAsync(Expression<Func<Exam, bool>> predicate,
                                           Func<IQueryable<Exam>, IIncludableQueryable<Exam, object>>? include = null)
        {
            IQueryable<Exam> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Exam>> GetAllAsync(Expression<Func<Exam, bool>>? predicate = null,
                                                          Func<IQueryable<Exam>, IIncludableQueryable<Exam, object>>? include = null,
                                                          Func<IQueryable<Exam>, IOrderedQueryable<Exam>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Exam> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Exam> Query(Expression<Func<Exam, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
