namespace Application.DTOs.GradeDTOs
{
    public class CreateGradeDto
    {
        public string GradeName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string CurriculumId { get; set; } = string.Empty; // Add this
    }
}
