using Application.DTOs.ParentDTOs;

namespace Application.DTOs.AuthDTOs
{
    public class ParentRegistrationResponse : AuthResponse
    {
        public ParentProfileDto ParentProfile { get; set; } = null!;
    }
}
