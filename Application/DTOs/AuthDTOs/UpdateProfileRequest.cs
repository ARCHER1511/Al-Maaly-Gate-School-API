using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.AuthDTOs
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
    }
}
