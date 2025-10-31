using Application.DTOs.ClassDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IClassService
    {
        Task<ServiceResult<IEnumerable<ClassViewDto>>> GetAllAsync();
        Task<ServiceResult<ClassViewDto>> GetByIdAsync(object id);
        Task<ServiceResult<ClassDto>> CreateAsync(ClassDto dto);
        Task<ServiceResult<ClassDto>> UpdateAsync(ClassDto dto);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
