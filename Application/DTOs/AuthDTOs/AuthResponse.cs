namespace Application.DTOs.AuthDTOs
{
    public class AuthResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public IList<string> Roles { get; set; } = new List<string>();
        public string? ProfileImageUrl { get; set; } // e.g. "/uploads/users/images/xxx.jpg"
        public Dictionary<string, string>? RoleEntityIds { get; set; }
        public string AccountStatus { get; set; } = string.Empty;
        public bool RequiresConfirmation { get; set; }
    }
}
