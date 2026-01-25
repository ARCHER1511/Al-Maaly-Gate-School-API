namespace Application.DTOs.ExamDTOS
{
    public class CreateExamWithQuestionsDto
    {
        public string ExamName { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }
        public string Status { get; set; } = string.Empty;

        // List of existing Question IDs to assign
        public List<string> QuestionIds { get; set; } = new();
    }
}
