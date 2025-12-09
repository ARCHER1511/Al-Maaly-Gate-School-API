using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : UserBase
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            // Inherit BaseEntity configuration
            new BaseEntityConfiguration<T>().Configure(builder);

            builder.Property(e => e.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.ContactInfo)
                   .HasMaxLength(500);

            builder.Property(e => e.AccountStatus)
                     .HasConversion(v => v.ToString(),
                                    v => (AccountStatus)Enum.Parse(typeof(AccountStatus), v))
                        .HasMaxLength(50)
                        .HasDefaultValue(AccountStatus.Pending);

            builder.HasOne(u => u.AppUser)
                   .WithMany()
                   .HasForeignKey(u => u.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.FullName)
                   .IsRequired()
                   .HasMaxLength(255);
        }
    }
}
