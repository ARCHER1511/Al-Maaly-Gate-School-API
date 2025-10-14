using System.Security.Claims;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AfterAuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AfterAuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            await _authService.RevokeTokensAsync(userId);
            return Ok("Logged out successfully");
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null)
                return Unauthorized();
            var user = await _authService.GetUserProfileAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found or not authenticated");

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (result.Success)
                return Ok(new { message = result.Data });

            return BadRequest(new { error = result.Message });
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var result = await _authService.DeleteAccountAsync(userId);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // extract the current userId from JWT claims
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await _authService.UpdateProfileAsync(userId, request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message, data = result.Data });
        }
    }
}
