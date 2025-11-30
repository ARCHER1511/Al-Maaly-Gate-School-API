using Application.DTOs.UserNotificationDTOs;
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
            var result = await _userNotificationService.GetByUserIdAsync(userId);
            if (!result.Success)
                return NotFound(ApiResponse<UserNotification>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<UserNotification>>.Ok(result.Data!,result.Message));
        }

        [HttpPut("mark-delivered")]
        public async Task<IActionResult> MarkAsDelivered([FromBody] MarkDeliveredDto dto)
        {
            var result = await _userNotificationService.MarkAsDeliveredAsync(dto.NotificationId, dto.UserId);
            return result.Success? Ok(ApiResponse<bool>.Ok(result.Data)) : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
    }
}
