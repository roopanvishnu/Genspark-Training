using Microsoft.AspNetCore.Mvc;
using ChienVHShopAPI.Dtos;
using ChienVHShopAPI.Interfaces;

namespace ChienVHShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactService;

        public ContactUsController(IContactUsService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContact([FromBody] ContactUsDto dto)
        {
            var result = await _contactService.SubmitContactFormAsync(dto);
            if (!result)
                return BadRequest("reCAPTCHA validation failed or input is invalid.");
            return Ok("Your message has been submitted successfully.");
        }
    }
}
