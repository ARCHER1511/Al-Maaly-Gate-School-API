using Application.DTOs.StudentDTOs;
using Domain.Wrappers;


namespace Application.Interfaces
{
     public interface IStudentService
    {
        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetAllAsync();
        Task<ServiceResult<StudentViewDto>> GetByIdAsync(object id);
        Task<ServiceResult<StudentViewDto>> CreateAsync(StudentViewDto dto);
        Task<ServiceResult<StudentViewDto>> UpdateAsync(StudentViewDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
        Task<ServiceResult<List<StudentSearchResultDto>>> SearchStudentsAsync(string searchTerm);
    }
}
