namespace Domain.Entities
{
    public class Question: BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? CorrectTextAnswer { get; set; }
        public decimal Degree { get; set; }
        public string? ExamId { get; set; }
        public Exam? Exam { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;
        public QuestionTypes Type { get; set; }
        public bool? TrueAndFalses { get; set; }
        public ChoiceAnswer? ChoiceAnswer { get; set; }
        public List<Choices>? Choices { get; set; } = new ();
    }

    public enum QuestionTypes
    {
        Complete,
        Connection,
        TrueOrFalse,
        Choices
    }
}