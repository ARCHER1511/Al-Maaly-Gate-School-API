using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Al_Maaly_Gate_School.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Success
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data))
                : BadRequest(ApiResponse<AuthResponse>.Fail(result.Message));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Success
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data))
                : Unauthorized(ApiResponse<AuthResponse>.Fail(result.Message));
        }
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await _authService.CreateRoleAsync(request);
            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message));
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var result = await _authService.AssignRoleAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _authService.RevokeTokensAsync(userId);
            return Ok("Logged out successfully");
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _authService.GetUserProfileAsync(userId);
            return Ok(user);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found or not authenticated");

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (result.Success)
                return Ok(new { message = result.Data });

            return BadRequest(new { error = result.Message });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            var result = await _authService.ForgotPasswordAsync(request.Email);

            if (!result.Success)
                return BadRequest(result.Message);

            // In production, don't return the token directly (used for testing or admin purpose)
            return Ok(new { message = result.Message, token = result.Data });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.DeleteAccountAsync(userId);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // extract the current userId from JWT claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await _authService.UpdateProfileAsync(userId, request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }
    }

}
