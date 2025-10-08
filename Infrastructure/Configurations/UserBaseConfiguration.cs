using Domain.Entities;
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

            builder.Property(e => e.IsApproved)
                   .IsRequired();

            builder.Property(e => e.AppUserId)
                   .IsRequired();
        }
    }
}
