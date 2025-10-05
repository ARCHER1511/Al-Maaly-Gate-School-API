using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ParentStudentRepository : IParentStudentRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<ParentStudent> _dbSet;

        public ParentStudentRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<ParentStudent>();
        }

        // 🔹 Ordinary methods
        public async Task<ParentStudent?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<ParentStudent>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(ParentStudent entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(ParentStudent entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(ParentStudent entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<ParentStudent?> GetAsync(Expression<Func<ParentStudent, bool>> predicate,
                                             Func<IQueryable<ParentStudent>, IIncludableQueryable<ParentStudent, object>>? include = null)
        {
            IQueryable<ParentStudent> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ParentStudent>> GetAllAsync(Expression<Func<ParentStudent, bool>>? predicate = null,
                                                          Func<IQueryable<ParentStudent>, IIncludableQueryable<ParentStudent, object>>? include = null,
                                                          Func<IQueryable<ParentStudent>, IOrderedQueryable<ParentStudent>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<ParentStudent> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<ParentStudent> Query(Expression<Func<ParentStudent, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
