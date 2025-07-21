using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notify.Models.DTOs;
using Notify.Services;

namespace Notify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        public AuthenticationController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO dto)
        {
            var result = await _authenticationService.Login(dto);
            return Ok(result);
        }
    }
}
