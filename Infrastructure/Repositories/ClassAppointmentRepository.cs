using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ClassAppointmentRepository : IClassAppointmentRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<ClassAppointment> _dbSet;

        public ClassAppointmentRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<ClassAppointment>();
        }

        // 🔹 Ordinary methods
        public async Task<ClassAppointment?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<ClassAppointment>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(ClassAppointment entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(ClassAppointment entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(ClassAppointment entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<ClassAppointment?> GetAsync(Expression<Func<ClassAppointment, bool>> predicate,
                                           Func<IQueryable<ClassAppointment>, IIncludableQueryable<ClassAppointment, object>>? include = null)
        {
            IQueryable<ClassAppointment> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ClassAppointment>> GetAllAsync(Expression<Func<ClassAppointment, bool>>? predicate = null,
                                                          Func<IQueryable<ClassAppointment>, IIncludableQueryable<ClassAppointment, object>>? include = null,
                                                          Func<IQueryable<ClassAppointment>, IOrderedQueryable<ClassAppointment>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<ClassAppointment> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<ClassAppointment> Query(Expression<Func<ClassAppointment, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
