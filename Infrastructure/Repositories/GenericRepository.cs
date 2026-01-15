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

        // 🔹 Ordinary methods - Make these virtual
        public virtual async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public virtual void Update(T entity) => _dbSet.Update(entity);
        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        // 🔹 LINQ-powered methods - Make these virtual too
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                       Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? predicate = null,
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

        public virtual IQueryable<T> AsQueryable(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }

        public virtual IQueryable<T> AsQueryable() => _dbSet.AsQueryable();

        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }


        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null!)
            => predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);

    }
}