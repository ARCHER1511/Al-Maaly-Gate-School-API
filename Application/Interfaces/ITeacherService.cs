using Application.DTOs.TeacherDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface ITeacherService
    {
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync();
    }
}
