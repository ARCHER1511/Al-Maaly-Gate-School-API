using System.Linq.Expressions;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.EntityFrameworkCore.Query;

namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface IAdminService
    {
        Task<ServiceResult<Admin>> GetByIdAsync(object id);
        Task<ServiceResult<IEnumerable<Admin>>> GetAllAsync();
        Task<ServiceResult<Admin?>> GetAsync(
            Expression<Func<Admin, bool>> predicate,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null
        );
        Task<ServiceResult<IEnumerable<Admin>>> GetAllAsync(
            Expression<Func<Admin, bool>>? predicate = null,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
            Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
            int? skip = null,
            int? take = null
        );
        Task<ServiceResult<Admin>> CreateAsync(Admin admin);
        Task<ServiceResult<Admin>> UpdateAsync(Admin admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
