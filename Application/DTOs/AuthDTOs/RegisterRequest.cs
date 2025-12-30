using Microsoft.AspNetCore.Http;

namespace Application.DTOs.AuthDTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ContactInfo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly BirthDay { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
