using Domain.Entities;

namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface IUserNotificationService
    {
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(string userId);
        Task<bool> MarkAsDeliveredAsync(string notificationId, string userId);
    }
}
