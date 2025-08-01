using System.Security.Claims;
using System.Text;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;
[ApiController]
[Route("api/[controller]")]
public class NewsManagementController : ControllerBase
{
    private NewsService _newsService;
    public NewsManagementController(NewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<News>>> GetAll()
    {
        try
        {
            var news = (await _newsService.GetAll()).ToList();
            return Ok(news);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<News>> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        try
        {
            var news = await _newsService.Get((int)id);
            return Ok(news);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
        
    }   
    [HttpPost("Create")]
    public async Task<ActionResult<News>> Create([FromBody] NewsAddDTO newsDTO)
    {
        try
        {
           int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            newsDTO.UserId = userId;
            var news = await _newsService.Create(newsDTO);
            return Ok(news);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<News>> Edit(int id,[FromBody] NewsAddDTO newsDTO)
    {
        try
        {
            var news = await _newsService.Edit(id,newsDTO);
            return Ok(news);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<News>> Delete(int id)
    {
        try
        {
            var news = await _newsService.Delete(id);
            return Ok(news);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
    [HttpPost("Export")]
    public async Task<ActionResult> ExportToCSV()
    {
        try
        {
            var newsContent = await _newsService.ExportContentToCSV();
            return File(Encoding.UTF8.GetBytes(newsContent), "text/csv", "export.csv");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
}