namespace Domain.Entities
{
    public class Student : UserBase
    {
        public string ClassYear { get; set; } = string.Empty;
        public int Age { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }

        public List<ParentStudent> Parents { get; set; } = new();
        public List<StudentSubjectExam> SubjectExams { get; set; } = new();
        public List<StudentQuestionAnswerExam> QuestionAnswers { get; set; } = new();
    }
}
