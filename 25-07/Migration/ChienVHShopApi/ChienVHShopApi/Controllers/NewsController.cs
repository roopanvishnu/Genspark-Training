using ChienVHShopAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedNews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 2)
    {
        var news = await _newsService.GetPagedNewsAsync(pageNumber, pageSize);
        return Ok(news);
    }
}
