using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface IAdminService
    {
        Task<Admin?> GetByIdAsync(object id);
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetAsync(Expression<Func<Admin, bool>> predicate,
                              Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null);
        Task<IEnumerable<Admin>> GetAllAsync(Expression<Func<Admin, bool>>? predicate = null,
                                             Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
                                             Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
                                             int? skip = null,
                                             int? take = null);
        Task<Admin> CreateAsync(Admin admin);
        Task<Admin> UpdateAsync(Admin admin);
        Task<bool> DeleteAsync(object id);
    }
}
