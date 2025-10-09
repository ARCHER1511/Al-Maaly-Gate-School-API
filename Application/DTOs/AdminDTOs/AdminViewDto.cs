namespace Application.DTOs.AdminDTOs
{
    public class AdminViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;

        public string AppUserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
