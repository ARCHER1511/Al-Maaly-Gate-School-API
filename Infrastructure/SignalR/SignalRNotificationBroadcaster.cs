using Domain.Entities;
using Domain.Interfaces.InfrastructureInterfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SignalR
{
    public class SignalRNotificationBroadcaster : INotificationBroadcaster
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<SignalRNotificationBroadcaster> _logger;

        public SignalRNotificationBroadcaster(
            IHubContext<NotificationHub> hubContext,
            ILogger<SignalRNotificationBroadcaster> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task BroadcastToUserAsync(string userId, Notification notification)
        {
            try
            {
                await _hubContext.Clients.User(userId)
                    .SendAsync("ReceiveNotification", Map(notification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification {Id} to user {UserId}", notification.Id, userId);
            }
        }

        public async Task BroadcastToUsersAsync(Notification notification, IEnumerable<string> userIds)
        {
            foreach (var userId in userIds)
                await BroadcastToUserAsync(userId, notification);
        }

        public async Task BroadcastToGroupAsync(string groupName, Notification notification)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", Map(notification));
        }

        public async Task BroadcastToAllAsync(Notification notification)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", Map(notification));
        }

        private object Map(Notification n)
        {
            return new
            {
                n.Id,
                n.Title,
                n.Message,
                n.Url,
                n.NotificationType,
                n.CreatedAt
            };
        }
    }
}
