using Application.DTOs.TeacherDTOs;
using Domain.Entities;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface ITeacherService
    {
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync();
        Task<ServiceResult<TeacherViewDto>> GetByIdAsync(string id);
        Task<ServiceResult<TeacherViewDto>> CreateAsync(CreateTeacherDto dto);
        Task<ServiceResult<TeacherViewDto>> UpdateAsync(string id, UpdateTeacherDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
    }
}
