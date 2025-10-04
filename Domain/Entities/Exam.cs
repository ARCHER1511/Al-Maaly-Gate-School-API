namespace Domain.Entities
{
    public class Exam
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = string.Empty;
        public decimal MinMark { get; set; } = 0;
        public decimal FullMark { get; set; } = 0;
        public string Link { get; set; } = string.Empty;
        public List<TeacherSubjectExam> TeacherSubjectExam { get; set; } = new List<TeacherSubjectExam>();
        public List<StudentSubjectExam> StudentSubjectExam { get; set; } = new List<StudentSubjectExam>();
        public List<QuestionExamTeacher> QuestionExamTeacher { get; set; } = new List<QuestionExamTeacher>();
        public List<StudentQuestionAnswerExam> StudentQuestionAnswerExam { get; set; } = new List<StudentQuestionAnswerExam>();



    }
}