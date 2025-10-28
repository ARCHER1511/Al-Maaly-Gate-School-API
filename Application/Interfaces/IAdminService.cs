using Application.DTOs.AdminDTOs;
using Application.DTOs.TeacherDTOs;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResult<AdminViewDto>> GetByIdAsync(object id);
        Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync();
        Task<ServiceResult<AdminViewDto?>> GetAsync(
            Expression<Func<Admin, bool>> predicate,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null
        );
        Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync(
            Expression<Func<Admin, bool>>? predicate = null,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
            Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
            int? skip = null,
            int? take = null
        );
        Task<ServiceResult<AdminViewDto>> CreateAsync(AdminCreateDto dto);
        Task<ServiceResult<AdminViewDto>> UpdateAsync(AdminUpdateDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
        Task<ServiceResult<int>> GetTeacherCount();
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeahcerInfo(string subjectName);
    }
}
