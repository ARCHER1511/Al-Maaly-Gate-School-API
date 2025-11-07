
namespace Domain.Entities
{
    public class StudentExamResult : BaseEntity
    {
        public decimal TotalMark { get; set; }
        public decimal FullMark { get; set; }
        public decimal MinMark { get; set; }
        public decimal Percentage { get; set; }
        public string Status { get; set; } = string.Empty;

        public string StudentId { get; set; } = string.Empty;
        public string ExamId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
    }
}
