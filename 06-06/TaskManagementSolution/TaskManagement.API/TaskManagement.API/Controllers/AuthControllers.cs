using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Auth;
using TaskManagement.API.Interfaces;

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
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        return Ok(new { success = true, message = "User registered", data = response });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(new { success = true, message = "Login successful", data = response });
    }

    [HttpGet("me")]
    [Authorize] // Add this attribute to require authentication
    public IActionResult Me()
    {
        // Use the same claim names as defined in the token generation
        var name = User.FindFirstValue("name") ?? "Unknown";
        var role = User.FindFirstValue("role") ?? "Unknown";
        var id = User.FindFirstValue("sub"); // 'sub' is the standard JWT claim for user ID
        var email = User.FindFirstValue("email");

        return Ok(new
        {
            success = true,
            message = "Current user",
            data = new { id, name, role, email }
        });
    }
}