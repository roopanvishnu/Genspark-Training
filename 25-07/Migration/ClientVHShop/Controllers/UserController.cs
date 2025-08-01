using System.Security.Claims;
using System.Text;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        try
        {
            var user = (await _userService.GetAll()).ToList();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<User>> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        try
        {
            var user = await _userService.Get((int)id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
        
    }   
    [HttpPost("Create")]
    public async Task<ActionResult<User>> Create([FromBody] UserDTO userDTO)
    {
        try
        {
            var user = await _userService.Create(userDTO);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    } 
    [Authorize]  
    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<User>> Edit(int id,[FromBody] UserDTO userDTO)
    {
        try
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (id != userId)
            {
                throw new Exception("UnAuthorized Access");
            }
            var user = await _userService.Edit(id,userDTO);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    } 
    [Authorize]  
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<User>> Delete(int id)
    {
        try
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (id != userId)
            {
                throw new Exception("UnAuthorized Access");
            }
            var user = await _userService.Delete(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
}