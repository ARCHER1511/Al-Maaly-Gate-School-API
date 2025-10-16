using System.Security.Claims;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Domain.Wrappers;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<string>.Fail("User not authenticated."));

            await _authService.RevokeTokensAsync(userId);
            return Ok(ApiResponse<string>.Ok(null!, "Logged out successfully."));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<AuthResponse>.Fail("User not authenticated."));

            var result = await _authService.GetUserProfileAsync(userId);

            return result.Success
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data!, result.Message))
                : NotFound(ApiResponse<AuthResponse>.Fail(result.Message!));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<string>.Fail("User not authenticated."));

            var result = await _authService.ChangePasswordAsync(userId, request);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<string>.Fail("User not authenticated."));

            var result = await _authService.DeleteAccountAsync(userId);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(null!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<string>.Fail("User not authenticated."));

            var result = await _authService.UpdateProfileAsync(userId, request);

            return result.Success
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<AuthResponse>.Fail(result.Message!));
        }
    }
}
