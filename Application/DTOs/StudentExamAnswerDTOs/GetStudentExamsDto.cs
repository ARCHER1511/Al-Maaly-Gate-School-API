using Domain.Entities;

namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class GetStudentExamsDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;
        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
