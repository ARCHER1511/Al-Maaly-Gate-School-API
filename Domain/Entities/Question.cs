namespace Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Content { get; set; }
        public string? CorrectAnswer { get; set; }
        public decimal Degree { get; set; }
        public bool IsRequired { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }
        public List<QuestionExamTeacher> QuestionExamTeachers { get; set; } = new List<QuestionExamTeacher>();
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public List<StudentQuestionAnswerExam> StudentQuestionAnswerExam { get; set; } = new List<StudentQuestionAnswerExam>();


    }
}