using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppRole : IdentityRole<string>
    {
        public AppRole() { }
        public ICollection<AppUserRole> UserRoles { get; set; } = new HashSet<AppUserRole>();
    }
}
