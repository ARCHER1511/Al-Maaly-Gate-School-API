namespace Application.DTOs.ParentDTOs
{
    public class StudentMinimalDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string? Relation { get; set; }
    }
}