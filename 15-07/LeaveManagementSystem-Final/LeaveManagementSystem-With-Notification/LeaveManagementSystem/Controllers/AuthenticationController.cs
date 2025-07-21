using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Import for ILogger
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthenticationController> _logger; 

        public AuthenticationController(IAuthenticationService authService, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login request received.");
                return BadRequest(new { message = "Invalid request data", errors = ModelState });
            }

            try
            {
                var response = await _authService.LoginAsync(request);
                _logger.LogInformation("User {Email} logged in successfully.", request.Email);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt for {Email}.", request.Email);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for {Email}.", request.Email);
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(refreshRequest.RefreshToken))
            {
                _logger.LogWarning("Invalid refresh token request received.");
                return BadRequest(new { message = "Refresh token is required" });
            }

            try
            {
                var newAccessToken = await _authService.RefreshTokenAsync(refreshRequest.RefreshToken);
                _logger.LogInformation("Refresh token generated successfully.");
                return Ok(new { AccessToken = newAccessToken });
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid refresh token attempt.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token refresh.");
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Logout attempt with invalid user identity.");
                    return Unauthorized(new { message = "Invalid user identity" });
                }

                await _authService.LogoutAsync(email);
                _logger.LogInformation("User {Email} logged out successfully.", email);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during logout.");
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync(User);
                _logger.LogInformation("Current user info retrieved successfully.");
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to user info.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving user info.");
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }
    }
}
