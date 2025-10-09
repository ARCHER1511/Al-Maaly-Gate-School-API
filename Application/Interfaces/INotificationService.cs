using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(
            string title,
            string message,
            string type,
            string? creatorUserId,
            IEnumerable<string> targetUserIds,
            string? url = null,
            string? role = null,
            bool isBroadcast = false
        );

        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<bool> MarkAsReadAsync(string notificationId, string userId);
    }
}
