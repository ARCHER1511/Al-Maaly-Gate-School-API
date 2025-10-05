namespace Domain.Entities
{
    public abstract class UserBase : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;

        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
    }
}
