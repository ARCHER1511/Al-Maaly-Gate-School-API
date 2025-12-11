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
        public string? GradeName { get; set; } = string.Empty;
        public string? CurriculumName { get; set; } = string.Empty; // Add this
        public string? CurriculumId { get; set; } = string.Empty; // Add this
        public int? Age { get; set; }
        public string? ClassId { get; set; } = string.Empty;
<<<<<<< HEAD
        public string? ClassYear { get; set; } = string.Empty; // Add this
        public string ProfileStatus { get; set; } = string.Empty;
=======
        public string AccountStatus { get; set; } = string.Empty;
>>>>>>> 103a6977b420bef1cbbc6beeffefa4218bd1bafa
        public List<ParentStudent>? Parents { get; set; } = new();
    }
}
