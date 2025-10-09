using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppRole : IdentityRole<string>
    {
        public AppRole() { }
        public List<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
