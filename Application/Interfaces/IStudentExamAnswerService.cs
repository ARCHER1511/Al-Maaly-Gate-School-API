using Application.DTOs.StudentExamAnswerDTOs;
using Domain.Wrappers;


namespace Application.Interfaces
{

    public interface IStudentExamAnswerService
    {
        //Task<ServiceResult<IEnumerable<GetStudentExamsDto>>> GetExams(string ClassId);
        Task<ServiceResult<IEnumerable<StudentExamAnswerDto>>> GetAllAsync();
        Task<ServiceResult<StudentExamAnswerDto>> GetByIdAsync(object id);
        Task<ServiceResult<StudentExamAnswerDto>> CreateAsync(StudentExamAnswerDto dto);
        Task<ServiceResult<StudentExamAnswerDto>> UpdateAsync(StudentExamAnswerDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
