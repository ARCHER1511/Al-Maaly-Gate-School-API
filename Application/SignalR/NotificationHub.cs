using Domain.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Application.SignalR
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IUserNotificationService _userNotificationService;

        public NotificationHub(ILogger<NotificationHub> logger, IUserNotificationService userNotificationService)
        {
            _logger = logger;
            _userNotificationService = userNotificationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Connected user has no identifier. ConnectionId: {ConnId}", Context.ConnectionId);
                await base.OnConnectedAsync();
                return;
            }

            _logger.LogInformation("User connected: {UserId}, Connection: {ConnId}", userId, Context.ConnectionId);

            // Optional: Add to role or project groups
            // await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            var undelivered = await _userNotificationService.GetByUserIdAsync(userId);

            foreach (var n in undelivered.Where(x => !x.IsDelivered))
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", Map(n.Notification));
                await _userNotificationService.MarkAsDeliveredAsync(n.NotificationId, userId);
            }


            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("User disconnected: {UserId}, Connection: {ConnId}", userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        // Direct send from clients (not often needed)
        public async Task SendToUser(string userId, object notification)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        public async Task Broadcast(object notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }

        private object Map(Notification notification)
        {
            return new
            {
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Url,
                notification.NotificationType,
                notification.CreatedAt
            };
        }
    }
}
