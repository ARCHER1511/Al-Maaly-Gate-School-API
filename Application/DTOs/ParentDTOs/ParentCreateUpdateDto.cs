using Domain.Enums;

namespace Application.DTOs.ParentDTOs
{
    public class ParentCreateUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; }
        public string? Gender { get; set; }
    }
}
