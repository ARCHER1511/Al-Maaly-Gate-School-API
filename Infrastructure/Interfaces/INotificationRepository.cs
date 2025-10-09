using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task MarkAsReadAsync(string notificationId, string userId);
        Task AddUserNotificationAsync(Notification notification, string userId);
    }
}
