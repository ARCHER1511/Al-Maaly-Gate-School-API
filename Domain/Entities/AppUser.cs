using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public AppUser() { }
        public string FullName { get; set; } = string.Empty;
        public List<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
        public List<UserNotification> UserNotifications { get; set; } = new();
    }
}
