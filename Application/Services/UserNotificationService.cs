using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.InfrastructureInterfaces;

namespace Application.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUserNotificationRepository _userNotificationRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UserNotificationService(IUserNotificationRepository userNotificationRepo, IUnitOfWork unitOfWork)
        {
            _userNotificationRepo = userNotificationRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(string userId)
        {
            return await _userNotificationRepo.GetByUserIdAsync(userId);
        }

        public async Task<bool> MarkAsDeliveredAsync(string notificationId, string userId)
        {
            var entity = await _userNotificationRepo.GetByNotificationAndUserAsync(notificationId, userId);
            if (entity == null)
                return false;

            entity.IsDelivered = true;
            entity.DeliveredAt = DateTime.Now;

            _userNotificationRepo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
