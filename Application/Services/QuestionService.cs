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

        public async Task<ServiceResult<IEnumerable<QuestionViewDto>>> GetAllAsync()
        {
            var questions = await _questionRepo.GetAllAsync();
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
                case QuestionTypes.Text:
                    if (string.IsNullOrWhiteSpace(dto.TextAnswer))
                        return ServiceResult<QuestionViewDto>.Fail("Text answer is required for text questions.");

                    question.TextAnswer = new TextAnswers
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = dto.TextAnswer.Trim(),
                        QuestionId = question.Id
                    };
                    break;

                case QuestionTypes.TrueOrFalse:
                    // Always create True and False choices
                    var trueChoice = new Choices
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = "True",
                        IsCorrect = false, // default
                        QuestionId = question.Id
                    };
                    var falseChoice = new Choices
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = "False",
                        IsCorrect = false, // default
                        QuestionId = question.Id
                    };

                    question.Choices = new List<Choices> { trueChoice, falseChoice };

                    // Normalize dto.CorrectChoiceId (it can be "true", "false", "True", "False", etc.)
                    var normalized = dto.CorrectChoiceId?.Trim().ToLower();

                    Choices correctChoice;
                    if (normalized == "true")
                        correctChoice = trueChoice;
                    else if (normalized == "false")
                        correctChoice = falseChoice;
                    else
                        return ServiceResult<QuestionViewDto>.Fail("CorrectChoiceId must be 'true' or 'false'.");

                    // Mark correct one
                    correctChoice.IsCorrect = true;

                    question.ChoiceAnswer = new ChoiceAnswer
                    {
                        Id = Guid.NewGuid().ToString(),
                        QuestionId = question.Id,
                        ChoiceId = correctChoice.Id
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
