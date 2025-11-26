using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

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

        public async Task<ServiceResult<IEnumerable<UserNotification>>> GetByUserIdAsync(string userId)
        {
            if (userId == null)
            return ServiceResult<IEnumerable<UserNotification>>.Fail("No user id added");
            var result = await _userNotificationRepo.GetByUserIdAsync(userId);
            return ServiceResult<IEnumerable<UserNotification>>.Ok(result,"notifications retrived successfully");
        }

        public async Task<ServiceResult<bool>> MarkAsDeliveredAsync(string notificationId, string userId)
        {
            var entity = await _userNotificationRepo.GetByNotificationAndUserAsync(notificationId, userId);
            if (entity == null)
                return ServiceResult<bool>.Fail("Failed To Update");

            entity.IsDelivered = true;
            entity.DeliveredAt = DateTime.Now;

            _userNotificationRepo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true,"Updated Successfully");
        }
    }
}
