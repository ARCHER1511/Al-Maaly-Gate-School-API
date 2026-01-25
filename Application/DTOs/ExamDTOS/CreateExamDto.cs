namespace Application.DTOs.ExamDTOS
{
    public class CreateExamDto
    {
        public string ExamName { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }
    }
}
