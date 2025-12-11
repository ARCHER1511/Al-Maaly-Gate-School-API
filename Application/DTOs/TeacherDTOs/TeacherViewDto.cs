using Domain.Enums;

namespace Application.DTOs.TeacherDTOs
{
    public class TeacherViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; }
        public List<string> Subjects { get; set; } = new();
        public List<string> ClassNames { get; set; } = new();
        public List<string> SpecializedCurricula { get; set; } = new(); // Add this
        public List<string> SpecializedCurriculumIds { get; set; } = new(); // Add this
    }

}
