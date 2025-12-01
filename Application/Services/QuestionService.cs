using Application.DTOs.QuestionDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

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

        public async Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync()
        {
            var questions = await _questionRepo.GetAllAsync();
            if (!questions.Any())
                return ServiceResult<IEnumerable<QuestionViewDto>>.Fail("No Questions found");
            var data = _mapper.Map<IEnumerable<QuestionViewDto>>(questions);
            return ServiceResult<IEnumerable<QuestionViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<QuestionViewDto>> GetByIdAsync(string id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
                return ServiceResult<QuestionViewDto>.Fail("Question not found");

            return ServiceResult<QuestionViewDto>.Ok(_mapper.Map<QuestionViewDto>(question));
        }

        public async Task<ServiceResult<QuestionViewDto>> CreateAsync(CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            question.Id = Guid.NewGuid().ToString();

            switch (dto.Type)
            {
                case QuestionTypes.Complete:
                    if (dto.CorrectTextAnswer == null)
                        return ServiceResult<QuestionViewDto>.Fail("Correct Text are required for questions.");
                    break;

                case QuestionTypes.TrueOrFalse:
                    break;

                case QuestionTypes.Connection:
                    if (dto.Choices == null || !dto.Choices.Any())
                        return ServiceResult<QuestionViewDto>.Fail("Choices are required for multiple-choice questions.");

                    var ConnectionChoices = dto.Choices.Select(c => new Choices
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = c.Text.Trim(),
                        IsCorrect = c.IsCorrect,
                        QuestionId = question.Id
                    }).ToList();

                    question.Choices = ConnectionChoices;

                    var correctConnection = ConnectionChoices.FirstOrDefault(c => c.IsCorrect);
                    if (correctConnection == null)
                        return ServiceResult<QuestionViewDto>.Fail("At least one choice must be marked as correct.");

                    question.ChoiceAnswer = new ChoiceAnswer
                    {
                        Id = Guid.NewGuid().ToString(),
                        QuestionId = question.Id,
                        ChoiceId = correctConnection.Id
                    };
                    break;

                case QuestionTypes.Choices:
                    if (dto.Choices == null || !dto.Choices.Any())
                        return ServiceResult<QuestionViewDto>.Fail("Choices are required for multiple-choice questions.");

                    var choices = dto.Choices.Select(c => new Choices
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = c.Text.Trim(),
                        IsCorrect = c.IsCorrect,
                        QuestionId = question.Id
                    }).ToList();

                    question.Choices = choices;

                    var correct = choices.FirstOrDefault(c => c.IsCorrect);
                    if (correct == null)
                        return ServiceResult<QuestionViewDto>.Fail("At least one choice must be marked as correct.");

                    question.ChoiceAnswer = new ChoiceAnswer
                    {
                        Id = Guid.NewGuid().ToString(),
                        QuestionId = question.Id,
                        ChoiceId = correct.Id
                    };
                    break;

                default:
                    return ServiceResult<QuestionViewDto>.Fail("Invalid question type.");
            }

            await _questionRepo.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<QuestionViewDto>(question);
            return ServiceResult<QuestionViewDto>.Ok(result, "Question created successfully");
        }


        public async Task<ServiceResult<QuestionViewDto>> UpdateAsync(string id, UpdateQuestionDto dto)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
                return ServiceResult<QuestionViewDto>.Fail("Question not found");

            _mapper.Map(dto, question);
            _questionRepo.Update(question);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<QuestionViewDto>.Ok(_mapper.Map<QuestionViewDto>(question), "Question updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
                return ServiceResult<bool>.Fail("Question not found");

            _questionRepo.Delete(question);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Question deleted successfully");
        }
    }
}
