using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AlMaalyGateSchoolContext context)
        {
            _context = context;
        }

        // Generic Repository Factory
        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepository<T>)_repositories[typeof(T)];

            var repoInstance = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repoInstance);
            return repoInstance;
        }

        // 🔥 CRITICAL: Add this method for queries
        public IQueryable<T> AsQueryable<T>() where T : BaseEntity
        {
            return _context.Set<T>().AsQueryable();
        }

        // 🔥 CRITICAL: Add this method for queries with filter
        public IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return _context.Set<T>().Where(predicate).AsQueryable();
        }

        // 🔥 CRITICAL: Add async methods for FirstOrDefault
        public async Task<T?> FirstOrDefaultAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
            where T : BaseEntity
        {
            var query = _context.Set<T>().AsQueryable();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        // 🔥 CRITICAL: Add async method for FindAll
        public async Task<List<T>> FindAllAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
            where T : BaseEntity
        {
            var query = _context.Set<T>().Where(predicate);

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        // 🔥 CRITICAL: Add async method for GetCount
        public async Task<int> GetCountAsync<T>(Expression<Func<T, bool>> predicate)
            where T : BaseEntity
        {
            return await _context.Set<T>().CountAsync(predicate);
        }

        // Identity repository
        public IGenericRepository<AppUser> AppUsers
            => new GenericRepository<AppUser>(_context);

        // Your specific repositories
        public IGenericRepository<Teacher> Teachers => Repository<Teacher>();
        public IGenericRepository<Student> Students => Repository<Student>();
        public IGenericRepository<Class> Classes => Repository<Class>();
        public IGenericRepository<Subject> Subjects => Repository<Subject>();
        public IGenericRepository<ClassAppointment> ClassAppointments => Repository<ClassAppointment>();
        public IGenericRepository<StudentExamAnswer> StudentExamAnswers => Repository<StudentExamAnswer>();
        public IGenericRepository<Grade> Grades => Repository<Grade>();
        public IGenericRepository<Certificate> Certificates => Repository<Certificate>();
        public IGenericRepository<Degree> Degrees => Repository<Degree>();
        public IGenericRepository<DegreeComponent> DegreesComponent => Repository<DegreeComponent>();
        public IGenericRepository<DegreeComponentType> DegreeComponentTypes => Repository<DegreeComponentType>();


        // Add other repositories as needed
        public IGenericRepository<Curriculum> Curriculums => Repository<Curriculum>();
        public IGenericRepository<Parent> Parents => Repository<Parent>();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}