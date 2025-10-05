using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ClassAssetsRepository : IClassAssetsRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<ClassAssets> _dbSet;

        public ClassAssetsRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<ClassAssets>();
        }

        // 🔹 Ordinary methods
        public async Task<ClassAssets?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<ClassAssets>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(ClassAssets entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(ClassAssets entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(ClassAssets entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<ClassAssets?> GetAsync(Expression<Func<ClassAssets, bool>> predicate,
                                           Func<IQueryable<ClassAssets>, IIncludableQueryable<ClassAssets, object>>? include = null)
        {
            IQueryable<ClassAssets> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ClassAssets>> GetAllAsync(Expression<Func<ClassAssets, bool>>? predicate = null,
                                                          Func<IQueryable<ClassAssets>, IIncludableQueryable<ClassAssets, object>>? include = null,
                                                          Func<IQueryable<ClassAssets>, IOrderedQueryable<ClassAssets>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<ClassAssets> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<ClassAssets> Query(Expression<Func<ClassAssets, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
