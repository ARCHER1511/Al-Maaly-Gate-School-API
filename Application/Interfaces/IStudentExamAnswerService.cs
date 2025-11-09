using Application.DTOs.StudentExamAnswerDTOs;
using Domain.Wrappers;


namespace Application.Interfaces
{

    public interface IStudentExamAnswerService 
    {
        Task<ServiceResult<StudentExamAnswerDto>> UpdateStudentTextAnswerMark(StudentExamAnswerDto dto);
        Task<ServiceResult<IEnumerable<StudentExamAnswerDto>>> GetExamsTextQuestions(string examId ,string subjectId, string ClassId);
        Task<ServiceResult<IEnumerable<GetStudentExamsDto>>> GetExamsForStudentByClassId(string classId);
        Task<ServiceResult<IEnumerable<StudentExamAnswerDto>>> GetAllAsync();
        Task<ServiceResult<StudentExamAnswerDto>> GetByIdAsync(object id);
        Task<ServiceResult<StudentExamAnswerDto>> CreateAsync(StudentExamAnswerDto dto);
        Task<ServiceResult<StudentExamAnswerDto>> UpdateAsync(StudentExamAnswerDto dto);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
