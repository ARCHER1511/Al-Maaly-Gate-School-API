namespace Application.DTOs.ParentDTOs
{
    public class ParentViewWithChildrenDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Relation { get; set; }
        public List<StudentMinimalDto> Students { get; set; } = new List<StudentMinimalDto>();
    }
}
