using Domain.Entities;

namespace Application.DTOs.StudentDTOs
{
    public class StudentViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? ClassName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? ContactInfo { get; set; } = string.Empty;
        public string? AppUserId { get; set; } = string.Empty;
        public string? ClassYear { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? ClassId { get; set; } = string.Empty;
        public string ProfileStatus { get; set; } = string.Empty;
        public List<ParentStudent>? Parents { get; set; } = new();
    }
}
