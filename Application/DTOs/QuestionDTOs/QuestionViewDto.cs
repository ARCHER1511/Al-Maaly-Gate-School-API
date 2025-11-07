using Domain.Entities;

namespace Application.DTOs.QuestionDTOs
{
    public class QuestionViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public decimal Degree { get; set; }
        public QuestionTypes Type { get; set; }
        public string TeacherId { get; set; } = string.Empty;

        // For questions that have choices
        public List<ChoiceViewDto>? Choices { get; set; }

        // For choice questions, which choice is correct
        public string? CorrectChoiceId { get; set; }

        public bool? TrueAndFalses { get; set; }

        // For text questions
        public string? TextAnswer { get; set; }
    }

    public class ChoiceViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
