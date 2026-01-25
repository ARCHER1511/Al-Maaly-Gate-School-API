namespace Application.DTOs.GradeDTOs
{
    public class GradeViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string CurriculumId { get; set; } = string.Empty; // Add this
        public string CurriculumName { get; set; } = string.Empty; // Add this
        public int ClassCount { get; set; }
        public int SubjectCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
