using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).IsRequired().HasMaxLength(255);
            builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.Url).HasMaxLength(500);
            builder.Property(n => n.NotificationType).HasMaxLength(100);
            builder.Property(n => n.Role).HasMaxLength(50);

            builder.HasOne(n => n.CreatorUser)
                   .WithMany()
                   .HasForeignKey(n => n.CreatorUserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable("Notifications", "Notifications");
        }
    }
}
