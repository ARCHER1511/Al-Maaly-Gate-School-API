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
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersByCurriculumAsync(string curriculumId);
        Task<ServiceResult<TeacherViewDto>> AddTeacherToCurriculumAsync(string teacherId, string curriculumId);
        Task<ServiceResult<TeacherViewDto>> RemoveTeacherFromCurriculumAsync(string teacherId, string curriculumId);
        Task<ServiceResult<int>> GetTeacherCountByCurriculumAsync(string curriculumId);
        Task<ServiceResult<TeacherDetailsDto>> GetTeacherDetailsAsync(string id);
    }
}
