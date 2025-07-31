using System.Security.Claims;
using System.Threading.Tasks;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private AuthService _authService;
    private UserService _userService;
    public AuthController(AuthService authService, UserService userService)
    {
        _authService = authService;
        _userService = userService;
    }


    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] UserDTO userDTO)
    {
        try
        {
            var res = await _authService.Login(userDTO);
            return res;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("Me")]
    public async Task<ActionResult<User>> Me()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.Get(int.Parse(userId!));
            return user;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}