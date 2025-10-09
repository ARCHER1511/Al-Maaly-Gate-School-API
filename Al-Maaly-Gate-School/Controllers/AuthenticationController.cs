using Application.DTOs.AuthDTOs;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

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
    }

}
