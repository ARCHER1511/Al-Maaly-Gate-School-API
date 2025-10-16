using Application.DTOs.QuestionDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class QuestionService : IQuestionService 
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepo, IUnitOfWork unitOfWork,IMapper mapper) 
        {
            _questionRepo = questionRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync() 
        {
            var questions = await _questionRepo.GetAllAsync();
            if (questions == null) 
            {
                return ServiceResult<IEnumerable<QuestionViewDto>>.Fail("No Question Found");
            }
            var dto = _mapper.Map<IEnumerable<QuestionViewDto>>(questions);
            return ServiceResult<IEnumerable<QuestionViewDto>>.Ok(dto, "Question Arrived");
        }

    }
}
