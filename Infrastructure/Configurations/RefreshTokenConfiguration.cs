using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            new BaseEntityConfiguration<RefreshToken>().Configure(builder);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Property(rt => rt.JwtId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(rt => rt.ExpiryDate)
                .IsRequired();

            builder.Property(rt => rt.IsUsed)
                .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                .IsRequired();

            builder.HasOne(rt => rt.AppUser)
                .WithMany()
                .HasForeignKey(rt => rt.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("RefreshTokens", "Identity");
        }
    }
}
