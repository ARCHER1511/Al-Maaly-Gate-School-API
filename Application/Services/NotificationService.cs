using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.InfrastructureInterfaces;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserNotificationRepository _userNotificationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationBroadcaster _broadcaster;

        public NotificationService(
            INotificationRepository notificationRepo,
            IUserNotificationRepository userNotificationRepo,
            IUnitOfWork unitOfWork,
            INotificationBroadcaster broadcaster)
        {
            _notificationRepo = notificationRepo;
            _userNotificationRepo = userNotificationRepo;
            _unitOfWork = unitOfWork;
            _broadcaster = broadcaster;
        }

        public async Task<Notification> CreateNotificationAsync(
            string title,
            string message,
            string type,
            string? creatorUserId,
            IEnumerable<string> targetUserIds,
            string? url = null,
            string? role = null,
            bool isBroadcast = false)
        {

            // Validate creator
            if (!string.IsNullOrWhiteSpace(creatorUserId))
            {
                var creatorExists = await _userNotificationRepo.UserExistsAsync(creatorUserId);
                if (!creatorExists)
                    throw new ArgumentException($"Creator user ID '{creatorUserId}' does not exist.");
            }

            // Validate targets
            foreach (var userId in targetUserIds)
            {
                var userExists = await _userNotificationRepo.UserExistsAsync(userId);
                if (!userExists)
                    throw new ArgumentException($"Target user ID '{userId}' does not exist.");
            }

            var notification = new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                Url = url ?? string.Empty,
                CreatorUserId = creatorUserId ?? string.Empty,
                Role = role,
                IsBroadcast = isBroadcast
            };

            await _notificationRepo.AddAsync(notification);

            foreach (var userId in targetUserIds)
            {
                await _notificationRepo.AddUserNotificationAsync(notification, userId);
            }

            await _unitOfWork.SaveChangesAsync();

            // Send via SignalR
            await _broadcaster.BroadcastToUsersAsync(notification, targetUserIds);

            return notification;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _notificationRepo.GetUserNotificationsAsync(userId);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _notificationRepo.GetUnreadNotificationsAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(string notificationId, string userId)
        {
            await _notificationRepo.MarkAsReadAsync(notificationId, userId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
