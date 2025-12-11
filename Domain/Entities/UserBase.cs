using Domain.Enums;

namespace Domain.Entities
{
    public abstract class UserBase : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactInfo { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
    }
}
