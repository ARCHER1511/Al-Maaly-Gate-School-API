using Application.DTOs.ExamDTOS;
using Application.DTOs.QuestionDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IQuestionService
    {
        Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync();
        Task<ServiceResult<QuestionViewDto>> GetByIdAsync(string id);
        Task<ServiceResult<QuestionViewDto>> CreateAsync(CreateQuestionDto dto);
        Task<ServiceResult<QuestionViewDto>> UpdateAsync(string id, UpdateQuestionDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
    }
}
