using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notify.Models;
using Notify.Models.DTOs;
using Notify.Services;

namespace Notify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult<ICollection<User>>> GetAll()
        {
            try
            {
                return Ok(await _userService.GetAll());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<User> AddUser(UserAddRequestDTO dto)
        {
            User user = await _userService.AddUser(dto);
            return user;
        }
    }
}
