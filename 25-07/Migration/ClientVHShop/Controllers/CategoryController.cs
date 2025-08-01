using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private CategoryService _categoryService;
    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetPageAsync(int? page)
    {
        try
        {
            var list = await _categoryService.GetPage(page);
            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("all")]
    public async Task<ActionResult<List<Category>>> GetAll()
    {
        try
        {
            var list = await _categoryService.GetAll();
            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Create")]
    public async Task<ActionResult<Category>> Create(string name)
    {
        try
        {
            var category = await _categoryService.Create(name);
            return Ok(category);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<Category>> Edit(int id, string name)
    {
        try
        {
            var category = await _categoryService.Edit(id, name);
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<Category>> Details(int id)
    {
        try
        {
            var category = await _categoryService.Get(id);
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<Category>> Delete(int id)
    {
        try
        {
            var category = await _categoryService.Delete(id);
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}