using Application.DTOs.StudentExamAnswerDTOs;
using Domain.Wrappers;


namespace Application.Interfaces
{

    public interface IStudentExamAnswerService 
    {
        Task<ServiceResult<List<StudentExamAnswerDto>>> SubmitExamAsync(StudentExamSubmissionDto submission);
        Task<ServiceResult<IEnumerable<GetStudentExamsDto>>> GetExamsForStudentByClassId(string classId);
        Task<ServiceResult<ExamQuestionsDto>> GetExamQuestions(string ExamId);
        Task<ServiceResult<IEnumerable<StudentExamAnswerDto>>> GetAllAsync();
        Task<ServiceResult<StudentExamAnswerDto>> GetByIdAsync(object id);
        Task<ServiceResult<StudentExamAnswerDto>> UpdateAsync(StudentExamAnswerDto dto);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
