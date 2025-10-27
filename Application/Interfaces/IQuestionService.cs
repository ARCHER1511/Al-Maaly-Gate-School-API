using Application.DTOs.ExamDTOS;
using Application.DTOs.QuestionDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IQuestionService
    {
        Task<ServiceResult<QuestionViewDto>> CreateQuestionAsync(string teacherId, CreateQuestionDto dto);
        Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync();
        Task<ServiceResult<QuestionViewDto>> GetByIdAsync(int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
