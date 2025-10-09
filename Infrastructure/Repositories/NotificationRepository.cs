using Infrastructure.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Notification> _dbSet;

        public NotificationRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Notification>();
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _dbSet
                .Include(n => n.UserNotifications)
                .Where(n => n.UserNotifications.Any(un => un.UserId == userId))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _dbSet
                .Include(n => n.UserNotifications)
                .Where(n => n.UserNotifications.Any(un => un.UserId == userId && !un.IsRead))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(string notificationId, string userId)
        {
            var userNotification = await _context.Set<UserNotification>()
                .FirstOrDefaultAsync(un => un.NotificationId == notificationId && un.UserId == userId);

            if (userNotification != null && !userNotification.IsRead)
            {
                userNotification.IsRead = true;
                userNotification.ReadAt = DateTime.Now;
                _context.Set<UserNotification>().Update(userNotification);
            }
        }

        public async Task AddUserNotificationAsync(Notification notification, string userId)
        {
            var userNotification = new UserNotification
            {
                NotificationId = notification.Id,
                UserId = userId
            };
            await _context.Set<UserNotification>().AddAsync(userNotification);
        }
    }
}
