namespace Application.DTOs.SubjectDTOs
{
    public class SubjectUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;

        public string? ClassId { get; set; }
        public string? TeacherId { get; set; }
    }
}
