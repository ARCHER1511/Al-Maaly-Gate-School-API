using Application.DTOs.NotificationDTOs;
using Common.Wrappers;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(ApiResponse<CreateNotificationDto>.Fail("Invalid notification data."));

            var notification = await _notificationService.CreateNotificationAsync(
                dto.Title,
                dto.Message,
                dto.Type,
                dto.CreatorUserId,
                dto.TargetUserIds,
                dto.Url,
                dto.Role,
                dto.IsBroadcast
            );

            return Ok(notification);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("user/{userId}/unread")]
        public async Task<IActionResult> GetUnreadNotifications(string userId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPut("mark-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkReadDto dto)
        {
            var result = await _notificationService.MarkAsReadAsync(dto.NotificationId, dto.UserId);
            return result ? Ok("Marked as read.") : BadRequest("Failed to mark as read.");
        }
    }
}

