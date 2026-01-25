using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
        public IGenericRepository<T> Repository<T>()
            where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepository<T>)_repositories[typeof(T)];

            var repoInstance = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repoInstance);
            return repoInstance;
        }

        // 🔥 CRITICAL: Add this method for queries
        public IQueryable<T> AsQueryable<T>()
            where T : BaseEntity
        {
            return _context.Set<T>().AsQueryable();
        }

        // 🔥 CRITICAL: Add this method for queries with filter
        public IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> predicate)
            where T : BaseEntity
        {
            return _context.Set<T>().Where(predicate).AsQueryable();
        }

        // 🔥 CRITICAL: Add async methods for FirstOrDefault
        public async Task<T?> FirstOrDefaultAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
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
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
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
        public IGenericRepository<AppUser> AppUsers => new GenericRepository<AppUser>(_context);

        // Your specific throught Generic repositories
        public IGenericRepository<Teacher> Teachers => Repository<Teacher>();
        public IGenericRepository<Student> Students => Repository<Student>();
        public IGenericRepository<Class> Classes => Repository<Class>();
        public IGenericRepository<Subject> Subjects => Repository<Subject>();
        public IGenericRepository<ClassAppointment> ClassAppointments =>
            Repository<ClassAppointment>();
        public IGenericRepository<StudentExamAnswer> StudentExamAnswers =>
            Repository<StudentExamAnswer>();
        public IGenericRepository<Grade> Grades => Repository<Grade>();
        public IGenericRepository<Certificate> Certificates => Repository<Certificate>();
        public IGenericRepository<Degree> Degrees => Repository<Degree>();
        public IGenericRepository<DegreeComponent> DegreesComponent =>
            Repository<DegreeComponent>();
        public IGenericRepository<DegreeComponentType> DegreeComponentTypes =>
            Repository<DegreeComponentType>();
        public IGenericRepository<Curriculum> Curriculums => Repository<Curriculum>();
        public IGenericRepository<Parent> Parents => Repository<Parent>();

        //Specific Repositories
        public IAdminRepository AdminRepository =>
            GetRepositories<IAdminRepository>(() => new AdminRepository(_context));
        public IAppUserRoleRepository AppUserRoleRepository =>
            GetRepositories<IAppUserRoleRepository>(() => new AppUserRoleRepository(_context));
        public IChoicesRepository ChoicesRepository =>
            GetRepositories<IChoicesRepository>(() => new ChoicesRepository(_context));
        public IClassAppointmentRepository ClassAppointmentRepository =>
            GetRepositories<IClassAppointmentRepository>(() =>
                new ClassAppointmentRepository(_context)
            );
        public IClassAssetsRepository ClassAssetsRepository =>
            GetRepositories<IClassAssetsRepository>(() => new ClassAssetsRepository(_context));
        public IClassRepository ClassRepository =>
            GetRepositories<IClassRepository>(() => new ClassRepository(_context));
        public ICurriculumRepository CurriculumRepository =>
            GetRepositories<ICurriculumRepository>(() => new CurriculumRepository(_context));
        public IExamRepository ExamRepository =>
            GetRepositories<IExamRepository>(() => new ExamRepository(_context));
        public IFileRecordRepository FileRecordRepository =>
            GetRepositories<IFileRecordRepository>(() => new FileRecordRepository(_context));
        public IGradeRepository GradeRepository =>
            GetRepositories<IGradeRepository>(() => new GradeRepository(_context));
        public INotificationRepository NotificationRepository =>
            GetRepositories<INotificationRepository>(() => new NotificationRepository(_context));
        public IParentRepository ParentRepository =>
            GetRepositories<IParentRepository>(() => new ParentRepository(_context));
        public IParentStudentRepository ParentStudentRepository =>
            GetRepositories<IParentStudentRepository>(() => new ParentStudentRepository(_context));
        public IQuestionRepository QuestionRepository =>
            GetRepositories<IQuestionRepository>(() => new QuestionRepository(_context));
        public IStudentExamAnswerRepository StudentExamAnswerRepository =>
            GetRepositories<IStudentExamAnswerRepository>(() =>
                new StudentExamAnswerRepository(_context)
            );
        public IStudentExamResultRepository StudentExamResultRepository =>
            GetRepositories<IStudentExamResultRepository>(() =>
                new StudentExamResultRepository(_context)
            );
        public IStudentRepository StudentRepository =>
            GetRepositories<IStudentRepository>(() => new StudentRepository(_context));
        public ISubjectRepository SubjectRepository =>
            GetRepositories<ISubjectRepository>(() => new SubjectRepository(_context));
        public ITeacherRepository TeacherRepository =>
            GetRepositories<ITeacherRepository>(() => new TeacherRepository(_context));
        public IUserNotificationRepository UserNotificationRepository =>
            GetRepositories<IUserNotificationRepository>(() =>
                new UserNotificationRepository(_context)
            );

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction == null)
                await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
                await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
                await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //Core Helpers
        private TRepo GetRepositories<TRepo>(Func<TRepo> factory)
            where TRepo : notnull
        {
            var type = typeof(TRepo);

            if (!_repositories.TryGetValue(type, out var repo))
            {
                repo = factory();
                _repositories[type] = repo;
            }
            return (TRepo)repo;
        }
    }
}
