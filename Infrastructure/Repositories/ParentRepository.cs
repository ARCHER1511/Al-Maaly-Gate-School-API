using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ParentRepository : IParentRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Parent> _dbSet;

        public ParentRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Parent>();
        }

        // 🔹 Ordinary methods
        public async Task<Parent?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Parent>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Parent entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Parent entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Parent entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Parent?> GetAsync(Expression<Func<Parent, bool>> predicate,
                                             Func<IQueryable<Parent>, IIncludableQueryable<Parent, object>>? include = null)
        {
            IQueryable<Parent> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Parent>> GetAllAsync(Expression<Func<Parent, bool>>? predicate = null,
                                                          Func<IQueryable<Parent>, IIncludableQueryable<Parent, object>>? include = null,
                                                          Func<IQueryable<Parent>, IOrderedQueryable<Parent>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Parent> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Parent> Query(Expression<Func<Parent, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
