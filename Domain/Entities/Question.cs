namespace Domain.Entities
{
    public class Question: BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? CorrectTextAnswer { get; set; }
        public decimal Degree { get; set; }
        public QuestionTypes Type { get; set; }
        public ICollection<Choices>? Choices { get; set; } = new HashSet<Choices>();
        public ChoiceAnswer? ChoiceAnswer { get; set; }
        public bool? TrueAndFalses { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new HashSet<ExamQuestion>();
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;
    }

    public enum QuestionTypes
    {
        Complete,
        Connection,
        TrueOrFalse,
        Choices
    }
}