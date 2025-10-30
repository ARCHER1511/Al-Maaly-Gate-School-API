namespace Application.DTOs.SubjectDTOs
{
    public class SubjectCreateDto
    {
        public string SubjectName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
    }
}
