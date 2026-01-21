using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Generic Repositories
        IGenericRepository<T> Repository<T>()
            where T : BaseEntity;
        IGenericRepository<AppUser> AppUsers { get; }
        IGenericRepository<Teacher> Teachers { get; }
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Class> Classes { get; }
        IGenericRepository<Subject> Subjects { get; }
        IGenericRepository<ClassAppointment> ClassAppointments { get; }
        IGenericRepository<StudentExamAnswer> StudentExamAnswers { get; }
        IGenericRepository<Grade> Grades { get; }
        IGenericRepository<Certificate> Certificates { get; }
        IGenericRepository<Degree> Degrees { get; }
        IGenericRepository<DegreeComponent> DegreesComponent { get; }
        IGenericRepository<DegreeComponentType> DegreeComponentTypes { get; }
        IGenericRepository<Curriculum> Curriculums { get; }
        IGenericRepository<Parent> Parents { get; }

        //Specific Reposiories
        IAdminRepository AdminRepository { get; }
        IAppUserRoleRepository AppUserRoleRepository { get; }
        IChoicesRepository ChoicesRepository { get; }
        IClassAppointmentRepository ClassAppointmentRepository { get; }
        IClassAssetsRepository ClassAssetsRepository { get; }
        IClassRepository ClassRepository { get; }
        ICurriculumRepository CurriculumRepository { get; }
        IExamRepository ExamRepository { get; }
        IFileRecordRepository FileRecordRepository { get; }
        IGradeRepository GradeRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IParentRepository ParentRepository { get; }
        IParentStudentRepository ParentStudentRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        IStudentExamAnswerRepository StudentExamAnswerRepository { get; }
        IStudentExamResultRepository StudentExamResultRepository { get; }
        IStudentRepository StudentRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ITeacherRepository TeacherRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        new void Dispose();

        IQueryable<T> AsQueryable<T>()
            where T : BaseEntity;
        IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> predicate)
            where T : BaseEntity;

        Task<T?> FirstOrDefaultAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
            where T : BaseEntity;

        Task<List<T>> FindAllAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
            where T : BaseEntity;

        Task<int> GetCountAsync<T>(Expression<Func<T, bool>> predicate)
            where T : BaseEntity;
    }
}
