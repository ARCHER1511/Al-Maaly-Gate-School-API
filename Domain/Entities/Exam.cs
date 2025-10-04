namespace Domain.Entities
{
    public class Exam
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = string.Empty;
        public Decimal MinMark { get; set; } = 0;
        public Decimal FullMark { get; set; } = 0;
        public string Link { get; set; } = string.Empty;
    }
}