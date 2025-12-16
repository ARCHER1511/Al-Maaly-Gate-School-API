using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IStudentService
    {
        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetAllAsync();
        Task<ServiceResult<StudentViewDto>> GetByIdAsync(object id);
        Task<ServiceResult<StudentViewDto>> CreateAsync(CreateStudentDto dto); // Changed parameter
        Task<ServiceResult<StudentViewDto>> UpdateAsync(string id, UpdateStudentDto dto); // Changed parameters
        Task<ServiceResult<bool>> DeleteAsync(object id);

        Task<ServiceResult<int>> GetStudentCountByCurriculumAsync(string curriculumId);
        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByCurriculumAsync(string curriculumId);
        Task<ServiceResult<StudentViewDto>> MoveStudentToCurriculumAsync(string studentId, string newCurriculumId);
        //Task<ServiceResult<List<StudentSearchResultDto>>> SearchStudentsAsync(string searchTerm);
        Task<ServiceResult<List<StudentSearchResultDto>>> SearchStudentsAsync(SearchTermDto dto);
    }
}