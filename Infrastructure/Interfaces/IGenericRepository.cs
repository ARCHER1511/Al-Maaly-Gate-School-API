using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // 🔹 Ordinary methods
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        // 🔹 LINQ-powered methods
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
                          Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? predicate = null,
                                         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                         Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                         int? skip = null,
                                         int? take = null);

        IQueryable<T> AsQueryable(Expression<Func<T, bool>>? predicate = null);
        IQueryable<T> AsQueryable();

        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
