using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ChienVHShopOnline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColorController : ControllerBase
{
    private ColorService _colorService;
    public ColorController(ColorService colorService)
    {
        _colorService = colorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Color>>> GetAll()
    {
        try
        {
            var colors = await _colorService.GetAll();
            return Ok(colors);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("Details/{id}")]
    public async Task<ActionResult<Color>> Get(int id)
    {
        try
        {
            var color = await _colorService.Get(id);
            return Ok(color);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpPost("Create")]
    public async Task<ActionResult<Color>> Create(string colorName)
    {
        try
        {
            var color = await _colorService.Create(colorName);
            return Ok(color);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<Color>> Edit(int id, string newColorName)
    {
        try
        {
            var color = await _colorService.Edit(id, newColorName);
            return Ok(color);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<Color>> Delete(int id)
    {
        try
        {
            var color = await _colorService.Delete(id);
            return Ok(color);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}