using Application.DTOs.UserNotificationDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class UserNotificationController : ControllerBase
    {
        private readonly IUserNotificationService _userNotificationService;

        public UserNotificationController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var notifications = await _userNotificationService.GetByUserIdAsync(userId);
            return Ok(notifications);
        }

        [HttpPut("mark-delivered")]
        public async Task<IActionResult> MarkAsDelivered([FromBody] MarkDeliveredDto dto)
        {
            var result = await _userNotificationService.MarkAsDeliveredAsync(dto.NotificationId, dto.UserId);
            return result ? Ok("Marked as delivered.") : BadRequest("Failed to mark as delivered.");
        }
    }
}
