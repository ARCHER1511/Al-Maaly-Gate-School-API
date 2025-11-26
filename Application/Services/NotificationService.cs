using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

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

        public async Task<ServiceResult<Notification>> CreateNotificationAsync(
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
                    ServiceResult<Notification>.Fail($"Creator user ID '{creatorUserId}' does not exist.");
            }

            // Validate targets
            foreach (var userId in targetUserIds)
            {
                var userExists = await _userNotificationRepo.UserExistsAsync(userId);
                if (!userExists)
                    ServiceResult<Notification>.Fail($"Target user ID '{userId}' does not exist.");
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

            return ServiceResult<Notification>.Ok(notification,"notification created sucessfully");
        }

        public async Task<ServiceResult<IEnumerable<Notification>>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _notificationRepo.GetUserNotificationsAsync(userId);
            if (notifications == null)
                return ServiceResult<IEnumerable<Notification>>.Fail("No notificaions found");
            return ServiceResult<IEnumerable<Notification>>.Ok(notifications,"Notifications retrived successfully");
        }

        public async Task<ServiceResult<IEnumerable<Notification>>> GetUnreadNotificationsAsync(string userId)
        {
            var unreadNotifications =  await _notificationRepo.GetUnreadNotificationsAsync(userId);
            if (unreadNotifications == null)
                return ServiceResult<IEnumerable<Notification>>.Fail("No Un Read notifaitions found");
            return ServiceResult<IEnumerable<Notification>>.Ok(unreadNotifications,"Un Read notifications retrived Successfully");
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(string notificationId, string userId)
        {
            var marked = await _notificationRepo.MarkAsReadAsync(notificationId, userId);
            if (!marked)
                ServiceResult<bool>.Fail("Failed to mark as read");
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(marked, "Marked Read Successfully");
        }
    }
}
