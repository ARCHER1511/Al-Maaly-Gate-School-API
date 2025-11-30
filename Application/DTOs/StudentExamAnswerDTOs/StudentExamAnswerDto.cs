using Domain.Entities;

namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class StudentExamAnswerDto
    {
        public string Id { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string ExamId { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public string? ChoiceId { get; set; }
        public bool? TrueAndFalseAnswer { get; set; }
        public string? CorrectTextAnswer { get; set; }
        public string? ConnectionLeftId { get; set; }
        public string? ConnectionRightId { get; set; }
        public decimal? Mark { get; set; }
    }

}
