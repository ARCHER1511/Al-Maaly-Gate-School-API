using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Class> _dbSet;

        public ClassRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Class>();
        }

        // 🔹 Ordinary methods
        public async Task<Class?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Class entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Class entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Class entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Class?> GetAsync(Expression<Func<Class, bool>> predicate,
                                           Func<IQueryable<Class>, IIncludableQueryable<Class, object>>? include = null)
        {
            IQueryable<Class> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Class>> GetAllAsync(Expression<Func<Class, bool>>? predicate = null,
                                                          Func<IQueryable<Class>, IIncludableQueryable<Class, object>>? include = null,
                                                          Func<IQueryable<Class>, IOrderedQueryable<Class>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Class> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Class> Query(Expression<Func<Class, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
