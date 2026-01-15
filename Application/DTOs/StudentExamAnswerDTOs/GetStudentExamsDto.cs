namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class GetStudentExamsDto
    {
        public string ExamId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }
        public string Status { get; set; } = "Upcoming";
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }
}
