namespace Domain.Entities
{
    public class StudentQuestionAnswerExam
    {
        public string StudentId { get; set; } = string.Empty;
        public Student? Student { get; set; }
        public int ExamId { get; set; } 
        public Exam? Exam { get; set; }
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        public int AnswerId { get; set; }
        public Answer? Answer { get; set; }

        


    }
}