namespace Domain.Entities
{
    public class Exam : BaseEntity
    {
        public string ExamName { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;
        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; } = "Upcoming";
        public decimal MinMark { get; set; } = 0;
        public decimal FullMark { get; set; } = 0;
        public List<Question> Questions { get; set; } = new();
    }
}