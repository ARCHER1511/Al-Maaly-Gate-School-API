using Application.DTOs.NotificationDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
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
                return BadRequest(ApiResponse<Notification>.Fail("Invalid notification data."));

            var result = await _notificationService.CreateNotificationAsync(
                dto.Title,
                dto.Message,
                dto.Type,
                dto.CreatorUserId,
                dto.TargetUserIds,
                dto.Url,
                dto.Role,
                dto.IsBroadcast
            );
            if (!result.Success)
                return BadRequest(ApiResponse<Notification>.Fail(result.Message!));
            return Ok(ApiResponse<Notification>.Ok(result.Data!, result.Message));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            var result = await _notificationService.GetUserNotificationsAsync(userId);
            if (!result.Success)
                return BadRequest(ApiResponse<IEnumerable<Notification>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<Notification>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("user/{userId}/unread")]
        public async Task<IActionResult> GetUnreadNotifications(string userId)
        {
            var result = await _notificationService.GetUnreadNotificationsAsync(userId);
            if (!result.Success)
                return BadRequest(ApiResponse<IEnumerable<Notification>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<Notification>>.Ok(result.Data!, result.Message));
        }

        [HttpPut("mark-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkReadDto dto)
        {
            var result = await _notificationService.MarkAsReadAsync(dto.NotificationId, dto.UserId);
            return result.Success ? Ok(ApiResponse<bool>.Ok(result.Data,result.Message)) : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
    }
}

