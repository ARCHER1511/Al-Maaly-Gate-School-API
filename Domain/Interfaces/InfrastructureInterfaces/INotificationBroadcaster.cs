using Domain.Entities;

namespace Domain.Interfaces.InfrastructureInterfaces
{
    public interface INotificationBroadcaster
    {
        Task BroadcastToUserAsync(string userId, Notification notification);
        Task BroadcastToUsersAsync(Notification notification, IEnumerable<string> userIds);
        Task BroadcastToGroupAsync(string groupName, Notification notification);
        Task BroadcastToAllAsync(Notification notification);
    }
}
