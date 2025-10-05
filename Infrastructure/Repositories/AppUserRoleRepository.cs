using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class AppUserRoleRepository : IAppUserRoleRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<AppUserRole> _dbSet;

        public AppUserRoleRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<AppUserRole>();
        }

        // 🔹 Ordinary methods
        public async Task<AppUserRole?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<AppUserRole>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(AppUserRole entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(AppUserRole entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(AppUserRole entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<AppUserRole?> GetAsync(Expression<Func<AppUserRole, bool>> predicate,
                                           Func<IQueryable<AppUserRole>, IIncludableQueryable<AppUserRole, object>>? include = null)
        {
            IQueryable<AppUserRole> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<AppUserRole>> GetAllAsync(Expression<Func<AppUserRole, bool>>? predicate = null,
                                                          Func<IQueryable<AppUserRole>, IIncludableQueryable<AppUserRole, object>>? include = null,
                                                          Func<IQueryable<AppUserRole>, IOrderedQueryable<AppUserRole>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<AppUserRole> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<AppUserRole> Query(Expression<Func<AppUserRole, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
