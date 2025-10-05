namespace Domain.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal MinMark { get; set; } = 0;
        public decimal FullMark { get; set; } = 0;
        public string Link { get; set; } = string.Empty;
        public List<TeacherSubjectExam> TeacherSubjectExams { get; set; } = new List<TeacherSubjectExam>();
        public List<StudentSubjectExam> StudentSubjectExams { get; set; } = new List<StudentSubjectExam>();
        public List<QuestionExamTeacher> QuestionExamTeachers { get; set; } = new List<QuestionExamTeacher>();
        public List<StudentQuestionAnswerExam> StudentQuestionAnswerExams { get; set; } = new List<StudentQuestionAnswerExam>();

    }
}