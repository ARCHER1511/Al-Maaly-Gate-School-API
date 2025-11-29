using Domain.Entities;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResult<Notification>> CreateNotificationAsync(
            string title,
            string message,
            string type,
            string? creatorUserId,
            IEnumerable<string> targetUserIds,
            string? url = null,
            string? role = null,
            bool isBroadcast = false
        );

        Task<ServiceResult<IEnumerable<Notification>>> GetUserNotificationsAsync(string userId);
        Task<ServiceResult<IEnumerable<Notification>>> GetUnreadNotificationsAsync(string userId);
        Task<ServiceResult<bool>> MarkAsReadAsync(string notificationId, string userId);
    }
}
