using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppRole : IdentityRole<string>
    {
        public List<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
