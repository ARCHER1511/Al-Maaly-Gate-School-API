namespace Domain.Entities
{
    public class QuestionExamTeacher
    {
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }

        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}
