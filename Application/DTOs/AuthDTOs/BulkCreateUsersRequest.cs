using Application.DTOs.AdminDTOs;

namespace Application.DTOs.AuthDTOs
{
    public class BulkCreateUsersRequest
    {
        public string UserType { get; set; } = string.Empty;
        public List<AdminCreateUserBaseDto> Users { get; set; } = new();
    }
}
