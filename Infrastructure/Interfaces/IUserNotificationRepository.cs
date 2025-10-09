using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(string userId);
        Task<UserNotification?> GetByNotificationAndUserAsync(string notificationId, string userId);
        Task<bool> UserExistsAsync(string userId);
    }
}
