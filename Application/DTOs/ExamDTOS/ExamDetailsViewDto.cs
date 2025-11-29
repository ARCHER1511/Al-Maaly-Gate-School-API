using Application.DTOs.QuestionDTOs;

namespace Application.DTOs.ExamDTOS
{
    public class ExamDetailsViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;

        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;

        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;

        public string TeacherId { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }
        public List<QuestionViewDto> Questions { get; set; } = new();
    }

}
