namespace Domain.Entities
{
    public class Question: BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public decimal Degree { get; set; }
        public string ExamId { get; set; } = string.Empty;
        public Exam? Exam { get; set; } = null!;
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;
        public TextAnswers? TextAnswer { get; set; }
        public TrueAndFalses? TrueAndFalses { get; set; } 
        public List<Choices>? Choices { get; set; } = new ();
    }
}