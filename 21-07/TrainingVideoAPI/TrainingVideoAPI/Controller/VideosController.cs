using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingVideoAPI.Data;
using TrainingVideoAPI.DTOs;
using TrainingVideoAPI.Services;

[ApiController]
[Route("api/videos")]
public class VideosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly BlobService _blobService;

    public VideosController(AppDbContext context, BlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] VideoUploadDto request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("No file uploaded.");

        var blobUrl = await _blobService.UploadAsync(request.File);

        var video = new TrainingVideo
        {
            Title = request.Title,
            Description = request.Description,
            BlobUrl = blobUrl,
            UploadDate = DateTime.UtcNow
        };

        _context.TrainingVideos.Add(video);
        await _context.SaveChangesAsync();

        return Ok(video);
    }

    [HttpGet]
    public async Task<IActionResult> GetVideos()
    {
        var videos = await _context.TrainingVideos
            .OrderByDescending(v => v.UploadDate)
            .ToListAsync();
        return Ok(videos);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        var video = await _context.TrainingVideos.FindAsync(id);
        if (video == null)
            return NotFound("Video not found.");

        var deletedFromBlob = await _blobService.DeleteAsync(video.BlobUrl);
        if (!deletedFromBlob)
            return StatusCode(500, "Failed to delete video from Blob Storage.");

        _context.TrainingVideos.Remove(video);
        await _context.SaveChangesAsync();

        return Ok("Video deleted successfully.");
    }


}
