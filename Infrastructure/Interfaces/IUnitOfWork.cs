using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        IGenericRepository<AppUser> AppUsers { get; }
        IGenericRepository<Teacher> Teachers { get; }
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Class> Classes { get; }
        IGenericRepository<Subject> Subjects { get; }
        IGenericRepository<ClassAppointment> ClassAppointments { get; }
        IGenericRepository<StudentExamAnswer> StudentExamAnswers { get; }
        IGenericRepository<Certificate> Certificates { get; }
        IGenericRepository<Degree> Degrees { get; }
        IGenericRepository<DegreeComponent> DegreesComponent { get; }
        IGenericRepository<DegreeComponentType> DegreeComponentTypes { get; }
        Task<int> SaveChangesAsync();

        IQueryable<T> AsQueryable<T>() where T : BaseEntity;
        IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        Task<T?> FirstOrDefaultAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
            where T : BaseEntity;

        Task<List<T>> FindAllAsync<T>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
            where T : BaseEntity;

        Task<int> GetCountAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;
    }
}
