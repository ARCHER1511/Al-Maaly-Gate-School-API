using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserNotificationService
    {
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(string userId);
        Task<bool> MarkAsDeliveredAsync(string notificationId, string userId);
    }
}
