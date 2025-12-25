using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public AppUser() { }
        public string FullName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string ProfileImagePath { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;
        public List<AppUserRole> UserRoles { get; set; } = new();
        public List<UserNotification> UserNotifications { get; set; } = new();
        public bool EmailConfirmed { get; set; }
        public string? ConfirmationNumber { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime? ConfirmationTokenExpiry { get; set; }
        public string? PendingRole { get; set; }
    }
}
