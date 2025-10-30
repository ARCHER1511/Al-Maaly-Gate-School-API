using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AlMaalyGateSchoolContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // 🔹 Ordinary methods
        public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);

        // 🔹 LINQ-powered methods
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                       Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? predicate = null,
                                                      Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                                      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                      int? skip = null,
                                                      int? take = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<T> AsQueryable(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
        public IQueryable<T> AsQueryable() => _dbSet.AsQueryable();
    }
}
