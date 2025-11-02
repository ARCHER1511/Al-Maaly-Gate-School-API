using Domain.Entities;

namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class StudentExamAnswerDto
    {
        public string Id { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        //public Student Student { get; set; } = null!;

        public string ExamId { get; set; } = string.Empty;
        //public Exam Exam { get; set; } = null!;

        public string QuestionId { get; set; } = string.Empty;
        //public Question Question { get; set; } = null!;

        public string? ChoiceId { get; set; }
        //public Choices? Choice { get; set; }

        public string? TrueAndFalseId { get; set; }
        //public TrueAndFalses? TrueAndFalse { get; set; }

        public string? TextAnswerId { get; set; }
        //public TextAnswers? TextAnswer { get; set; }
        public decimal? Mark { get; set; }
    }
}
