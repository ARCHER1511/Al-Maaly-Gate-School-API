using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<UserNotification> _dbSet;

        public UserNotificationRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<UserNotification>();
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(un => un.Notification)
                .Where(un => un.UserId == userId)
                .OrderByDescending(un => un.Notification.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserNotification?> GetByNotificationAndUserAsync(string notificationId, string userId)
        {
            return await _dbSet
                .Include(un => un.Notification)
                .FirstOrDefaultAsync(un => un.NotificationId == notificationId && un.UserId == userId);
        }
        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
