namespace Application.DTOs.SubjectDTOs
{
    public class SubjectViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public double CreditHours { get; set; }
        public int TeacherCount { get; set; }
        public int ExamCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // REMOVED: TeacherId, TeacherName (these come from TeacherSubject relationship)
    }
}
