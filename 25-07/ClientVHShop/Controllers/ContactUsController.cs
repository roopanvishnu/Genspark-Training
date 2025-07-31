using System.Text;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactUsController : ControllerBase
{
    private ContactUsService _contactUsService;
    public ContactUsController(ContactUsService contactUsService)
    {
        _contactUsService = contactUsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContactU>>> GetAll()
    {
        try
        {
            var ContactUs = (await _contactUsService.GetAll()).ToList();
            return Ok(ContactUs);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<ContactU>> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        try
        {
            var ContactUs = await _contactUsService.Get((int)id);
            return Ok(ContactUs);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }

    }
    [HttpPost("Create")]
    public async Task<ActionResult<ContactU>> Create([FromBody] ContactUsAddDTO ContactUsDTO)
    {
        try
        {
            var ContactUs = await _contactUsService.Create(ContactUsDTO);
            return Ok(ContactUs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<ContactU>> Edit(int id, [FromBody] ContactUsAddDTO ContactUsDTO)
    {
        try
        {
            var ContactUs = await _contactUsService.Edit(id, ContactUsDTO);
            return Ok(ContactUs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<ContactU>> Delete(int id)
    {
        try
        {
            var ContactUs = await _contactUsService.Delete(id);
            return Ok(ContactUs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}