using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<Student> _dbSet;

        public StudentRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<Student>();
        }

        // 🔹 Ordinary methods
        public async Task<Student?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Student entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Student entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Student entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<Student?> GetAsync(Expression<Func<Student, bool>> predicate,
                                             Func<IQueryable<Student>, IIncludableQueryable<Student, object>>? include = null)
        {
            IQueryable<Student> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Student>> GetAllAsync(Expression<Func<Student, bool>>? predicate = null,
                                                          Func<IQueryable<Student>, IIncludableQueryable<Student, object>>? include = null,
                                                          Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<Student> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<Student> Query(Expression<Func<Student, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
