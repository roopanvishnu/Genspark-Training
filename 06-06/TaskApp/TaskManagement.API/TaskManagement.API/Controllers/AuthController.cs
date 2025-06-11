using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result)
                return BadRequest(new { success = false, message = "User already exists." });

            return Ok(new { success = true, message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { success = false, message = "Invalid email or password" });

            return Ok(new
            {
                success = true,
                message = "Login successful",
                token
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _authService.GetCurrentUserAsync(HttpContext);
            if (user == null)
                return Unauthorized(new { success = false, message = "Unauthorized" });

            return Ok(new
            {
                success = true,
                message = "User details",
                data = user
            });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result == null)
                return Unauthorized(new { success = false, message = "Invalid or expired refresh token" });

            return Ok(new
            {
                success = true,
                message = "Token refreshed successfully",
                accessToken = result.Value.accessToken,
                refreshToken = result.Value.refreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true, message = "Logged out successfully" });
        }

        
    }
}