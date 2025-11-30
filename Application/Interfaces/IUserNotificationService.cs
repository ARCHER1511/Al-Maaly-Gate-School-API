using Domain.Entities;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IUserNotificationService
    {
        Task<ServiceResult<IEnumerable<UserNotification>>> GetByUserIdAsync(string userId);
        Task<ServiceResult<bool>> MarkAsDeliveredAsync(string notificationId, string userId);
    }
}
