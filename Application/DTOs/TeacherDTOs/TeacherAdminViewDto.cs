using Domain.Entities;
using System.Collections;

namespace Application.DTOs.TeacherDTOs
{
    public class TeacherAdminViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public List<string> Subjects = new List<string>();
        public List<string> ClassNames = new List<string>();
        public string ProfileStatus { get; set; } = string.Empty;
    }
}
