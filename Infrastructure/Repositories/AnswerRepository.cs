using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Answer> _dbSet;

        public AnswerRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Answer>();
        }

        // 🔹 Ordinary methods
        public async Task<Answer?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Answer>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Answer entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Answer entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Answer entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Answer?> GetAsync(Expression<Func<Answer, bool>> predicate,
                                           Func<IQueryable<Answer>, IIncludableQueryable<Answer, object>>? include = null)
        {
            IQueryable<Answer> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Answer>> GetAllAsync(Expression<Func<Answer, bool>>? predicate = null,
                                                          Func<IQueryable<Answer>, IIncludableQueryable<Answer, object>>? include = null,
                                                          Func<IQueryable<Answer>, IOrderedQueryable<Answer>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Answer> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Answer> Query(Expression<Func<Answer, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
