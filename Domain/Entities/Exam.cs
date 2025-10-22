namespace Domain.Entities
{
    public class Exam
    {
        public int Id { get; set; }

        // From DTOs
        public string Title { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public int DurationMinutes { get; set; }

        // Existing fields
        public string Type { get; set; } = string.Empty;
        public decimal MinMark { get; set; } = 0;
        public decimal FullMark { get; set; } = 0;
        public string Link { get; set; } = string.Empty;

        public List<TeacherSubjectExam> TeacherSubjectExams { get; set; } = new();
        public List<StudentSubjectExam> StudentSubjectExams { get; set; } = new();
        public List<QuestionExamTeacher> QuestionExamTeachers { get; set; } = new();
        public List<StudentQuestionAnswerExam> StudentQuestionAnswerExams { get; set; } = new();
    }
}