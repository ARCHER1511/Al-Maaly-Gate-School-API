using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(un => un.Id);

            builder.HasOne(un => un.Notification)
                   .WithMany(n => n.UserNotifications)
                   .HasForeignKey(un => un.NotificationId);

            builder.HasOne(un => un.User)
                   .WithMany(u => u.UserNotifications)
                   .HasForeignKey(un => un.UserId);
        }
    }
}
    