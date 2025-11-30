using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Al_Maaly_Gate_School.Seeding
{
    public static class IdentitySeeder
    {
        private static readonly string[] Roles = { "Admin", "Teacher", "Student", "Parent" };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            //Seed Roles
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new AppRole { Id = Guid.NewGuid().ToString(), Name = role }
                    );
                }
            }
            // Seed Admin
            string adminEmail = "admin@gate.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded)
                {
                    throw new Exception(
                        "Failed to create admin user: "
                            + string.Join(", ", createResult.Errors.Select(e => e.Description))
                    );
                }

                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                // Ensure admin is in role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Admin domain entity
            bool adminExists = await ctx.Admins.AnyAsync(a => a.AppUserId == adminUser.Id);

            if (!adminExists)
            {
                var adminEntity = new Admin
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = adminUser.FullName,
                    Email = adminUser.Email!,
                    ContactInfo = "",
                    AppUserId = adminUser.Id,
                    Type = "SuperAdmin",
                    ProfileStatus = Domain.Enums.ProfileStatus.Approved,
                };
                ctx.Admins.Add(adminEntity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
