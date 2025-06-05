using System.Security.Claims;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = FirstAPI.Interfaces.IAuthenticationService;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public GoogleAuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("GoogleCallback", "GoogleAuth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<ActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = result.Principal;
            
            // foreach (var c in claims)
            //     System.Console.WriteLine(c.Type + "\t" + c.Value);

            var email = claimsPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
            var name = claimsPrincipal?.FindFirst(ClaimTypes.Name)?.Value;
            var nameId = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            GoogleLoginDTO dto = new GoogleLoginDTO { Email = email, Name = name, NameId = nameId };
            try
            {
                dto = await _authenticationService.LoginWithGoogle(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (dto.Token == null)
            {
                return NotFound("No user found with registered email");
            }
            return Ok(dto);
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

    }
}