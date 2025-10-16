using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [AllowAnonymous]
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
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<AuthResponse>.Fail(result.Message!));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Success
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data!, result.Message))
                : Unauthorized(ApiResponse<AuthResponse>.Fail(result.Message!));
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await _authService.CreateRoleAsync(request);
            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var result = await _authService.AssignRoleAsync(request);
            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }

        [HttpPost("unassign-role")]
        public async Task<IActionResult> UnassignRole([FromBody] AssignRoleRequest request)
        {
            var result = await _authService.UnassignRoleAsync(request);
            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return result.Success
                ? Ok(ApiResponse<AuthResponse>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<AuthResponse>.Fail(result.Message!));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(ApiResponse<string>.Fail("Email is required."));

            var result = await _authService.ForgotPasswordAsync(request.Email);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid input data."));

            var result = await _authService.ResetPasswordAsync(request);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message!));
        }
    }
}
