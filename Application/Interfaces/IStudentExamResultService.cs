using Application.DTOs.StudentExamAnswerDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IStudentExamResultService
    {
        Task<ServiceResult<IEnumerable<StudentExamResultDto>>> GetAllResultsByStudentIdAsync(object Id);
        Task<ServiceResult<IEnumerable<StudentExamResultDto>>> GetAllAsync();
        Task<ServiceResult<StudentExamResultDto>> GetByIdAsync(object id);
        Task<ServiceResult<StudentExamResultDto>> CreateAsync(StudentExamResultDto dto);
        Task<ServiceResult<StudentExamResultDto>> UpdateAsync(StudentExamResultDto dto);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
