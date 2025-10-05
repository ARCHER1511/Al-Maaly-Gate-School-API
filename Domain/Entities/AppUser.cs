using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public List<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
