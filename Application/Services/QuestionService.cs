using Application.DTOs.QuestionDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionRepo = questionRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<QuestionViewDto>> CreateQuestionAsync(string teacherId, CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            question.TeacherId = teacherId;

            await _questionRepo.AddAsync(question);
            await _unitOfWork.SaveChangesAsync(); // ✅ persist to DB

            var result = _mapper.Map<QuestionViewDto>(question);
            return ServiceResult<QuestionViewDto>.Ok(result, "Question created successfully");
        }

        public async Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync()
        {
            var questions = await _questionRepo.GetAllAsync();
            var data = questions.Select(_mapper.Map<QuestionViewDto>);
            return ServiceResult<IEnumerable<QuestionViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<QuestionViewDto>> GetByIdAsync(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
                return ServiceResult<QuestionViewDto>.Fail("Question not found");

            var data = _mapper.Map<QuestionViewDto>(question);
            return ServiceResult<QuestionViewDto>.Ok(data);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
                return ServiceResult<bool>.Fail("Question not found");

            _questionRepo.Delete(question);
            await _unitOfWork.SaveChangesAsync(); // ✅ persist deletion

            return ServiceResult<bool>.Ok(true, "Question deleted successfully");
        }
    }
}
