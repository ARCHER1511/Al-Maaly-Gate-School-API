using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Admin> _dbSet;

        public AdminRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Admin>();
        }

        // 🔹 Ordinary methods
        public async Task<Admin?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Admin entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Admin entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Admin entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Admin?> GetAsync(Expression<Func<Admin, bool>> predicate,
                                           Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null)
        {
            IQueryable<Admin> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync(Expression<Func<Admin, bool>>? predicate = null,
                                                          Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
                                                          Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Admin> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Admin> Query(Expression<Func<Admin, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}

