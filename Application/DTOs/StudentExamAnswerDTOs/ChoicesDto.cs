using Domain.Entities;

namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class ChoicesDto
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
