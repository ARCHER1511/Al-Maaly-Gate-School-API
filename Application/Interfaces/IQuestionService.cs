using Application.DTOs.QuestionDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IQuestionService
    {
        Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync();
    }
}
