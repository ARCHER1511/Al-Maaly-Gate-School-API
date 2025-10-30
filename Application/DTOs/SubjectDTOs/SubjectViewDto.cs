namespace Application.DTOs.SubjectDTOs
{
    public class SubjectViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;

        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;

        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;

        public int ExamCount { get; set; }
    }
}
