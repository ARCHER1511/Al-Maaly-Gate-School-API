using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ClassSubjectRepository : IClassSubjectRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<ClassSubject> _dbSet;

        public ClassSubjectRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<ClassSubject>();
        }

        // 🔹 Ordinary methods
        public async Task<ClassSubject?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<ClassSubject>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(ClassSubject entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(ClassSubject entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(ClassSubject entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<ClassSubject?> GetAsync(Expression<Func<ClassSubject, bool>> predicate,
                                           Func<IQueryable<ClassSubject>, IIncludableQueryable<ClassSubject, object>>? include = null)
        {
            IQueryable<ClassSubject> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ClassSubject>> GetAllAsync(Expression<Func<ClassSubject, bool>>? predicate = null,
                                                          Func<IQueryable<ClassSubject>, IIncludableQueryable<ClassSubject, object>>? include = null,
                                                          Func<IQueryable<ClassSubject>, IOrderedQueryable<ClassSubject>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<ClassSubject> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<ClassSubject> Query(Expression<Func<ClassSubject, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
