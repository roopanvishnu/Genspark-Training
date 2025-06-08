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
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var name = User.Identity?.Name ?? "Unknown";
        var role = User.IsInRole("Manager") ? "Manager" : "TeamMember";
        return Ok(new { success = true, message = "Current user", data = new { name, role } });
    }
}