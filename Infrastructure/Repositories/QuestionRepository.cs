using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Question> _dbSet;

        public QuestionRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Question>();
        }

        // 🔹 Ordinary methods
        public async Task<Question?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Question entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Question entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Question entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Question?> GetAsync(Expression<Func<Question, bool>> predicate,
                                             Func<IQueryable<Question>, IIncludableQueryable<Question, object>>? include = null)
        {
            IQueryable<Question> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Question>> GetAllAsync(Expression<Func<Question, bool>>? predicate = null,
                                                          Func<IQueryable<Question>, IIncludableQueryable<Question, object>>? include = null,
                                                          Func<IQueryable<Question>, IOrderedQueryable<Question>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Question> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Question> Query(Expression<Func<Question, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }

    }
}
