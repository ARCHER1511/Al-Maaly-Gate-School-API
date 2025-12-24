using Application.DTOs.AuthDTOs;

namespace Application.DTOs.ParentDTOs
{
    public class ParentRegisterRequest : RegisterRequest
    {
        public string? Relation { get; set; }
    }
}
