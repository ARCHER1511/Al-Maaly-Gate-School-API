using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        IGenericRepository<AppUser> AppUsers { get; }
        IGenericRepository<Teacher> Teachers { get; }
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Class> Classes { get; }
        IGenericRepository<Subject> Subjects { get; }
        IGenericRepository<ClassAppointment> ClassAppointments { get; }
        IGenericRepository<StudentExamAnswer> StudentExamAnswers { get; }
        Task<int> SaveChangesAsync();
    }
}
