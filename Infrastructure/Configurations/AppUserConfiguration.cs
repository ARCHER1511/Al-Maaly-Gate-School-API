using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.FullName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(u => u.ContactInfo)
                .HasMaxLength(512);

            builder.Property(u => u.ProfileImagePath)
                   .HasMaxLength(300);

            builder.Property(u => u.Gender)
               .HasMaxLength(20)
               .IsRequired(true);

            builder.Property(u => u.Age)
                   .IsRequired(true);

            //Enum as string
            builder.Property(u => u.AccountStatus)
                .HasConversion(v => v.ToString(),
                               v => (AccountStatus)Enum.Parse(typeof(AccountStatus), v))
                .HasMaxLength(50)
                .HasDefaultValue(AccountStatus.Pending);

            builder.HasMany(u => u.UserRoles)
                   .WithOne(ur => ur.User)
                   .HasForeignKey(ur => ur.UserId);

            builder.HasMany(u => u.UserNotifications)
                   .WithOne(un => un.User)
                   .HasForeignKey(un => un.UserId);

            builder.ToTable("AppUsers", "Identity");
        }
    }
}
