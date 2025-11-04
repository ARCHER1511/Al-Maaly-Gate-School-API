
namespace Domain.Entities
{
    public class StudentExamAnswer : BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;

        public string ExamId { get; set; } = string.Empty;
        public Exam Exam { get; set; } = null!;

        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;

        public string? ChoiceId { get; set; }
        public bool? TrueAndFalseAnswer { get; set; }
        public string? TextAnswer { get; set; }
        public decimal? Mark { get; set; }
    }
}
