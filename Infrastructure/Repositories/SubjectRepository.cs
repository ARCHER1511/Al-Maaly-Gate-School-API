using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Subject> _dbSet;

        public SubjectRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Subject>();
        }

        // 🔹 Ordinary methods
        public async Task<Subject?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Subject entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Subject entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Subject entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Subject?> GetAsync(Expression<Func<Subject, bool>> predicate,
                                             Func<IQueryable<Subject>, IIncludableQueryable<Subject, object>>? include = null)
        {
            IQueryable<Subject> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Subject>> GetAllAsync(Expression<Func<Subject, bool>>? predicate = null,
                                                          Func<IQueryable<Subject>, IIncludableQueryable<Subject, object>>? include = null,
                                                          Func<IQueryable<Subject>, IOrderedQueryable<Subject>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Subject> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Subject> Query(Expression<Func<Subject, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
