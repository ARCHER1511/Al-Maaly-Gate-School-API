namespace Domain.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public bool IsCorrect { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }
        public string QuestionId { get; set; } = string.Empty;
        public Question? Question { get; set; }
        public List<StudentQuestionAnswerExam> StudentQuestionAnswerExam { get; set; } = new List<StudentQuestionAnswerExam>();


    }
}
