namespace Domain.Entities
{
    public class ExamQuestion : BaseEntity
    {
        public string ExamId { get; set; } = string.Empty;
        public Exam Exam { get; set; } = null!;

        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;
    }
}
