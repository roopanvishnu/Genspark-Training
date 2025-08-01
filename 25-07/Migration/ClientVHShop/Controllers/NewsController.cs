using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private NewsService _newsService;
    public NewsController(NewsService newsService)
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
    [HttpGet("page/{page}")]
    public async Task<ActionResult<List<News>>> GetPage(int? page)
    {
        try
        {
            var news = (await _newsService.GetPage(page)).ToList();
            return Ok(news);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}