using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;

namespace Domain.Interfaces.InfrastructureInterfaces
{
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(string userId);
        Task<UserNotification?> GetByNotificationAndUserAsync(string notificationId, string userId);
        Task<bool> UserExistsAsync(string userId);
    }
}
