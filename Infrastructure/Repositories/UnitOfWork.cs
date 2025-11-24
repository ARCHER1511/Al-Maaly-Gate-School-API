using Infrastructure.Interfaces;
using Infrastructure.Data;
using Domain.Entities;

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

        // Identity repository (NOT BaseEntity)
        public IGenericRepository<AppUser> AppUsers
            => new GenericRepository<AppUser>(_context);

        public IGenericRepository<Teacher> Teachers => Repository<Teacher>();
        public IGenericRepository<Student> Students => Repository<Student>();
        public IGenericRepository<Class> Classes => Repository<Class>();
        public IGenericRepository<Subject> Subjects => Repository<Subject>();
        public IGenericRepository<ClassAppointment> ClassAppointments => Repository<ClassAppointment>();
        public IGenericRepository<StudentExamAnswer> StudentExamAnswers => Repository<StudentExamAnswer>();

        // 🔥 FIXED: You forgot these
        public IGenericRepository<Certificate> Certificates => Repository<Certificate>();
        public IGenericRepository<Degree> Degrees => Repository<Degree>();

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
