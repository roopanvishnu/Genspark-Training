using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using TaskManagement.API.Context;


namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackupController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public BackupController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("task-attachments")]
    public async Task<IActionResult> BackupTaskAttachments()
    {
        var attachments = await _context.TaskAttachments
            .AsNoTracking()
            .Include(a => a.TaskItem)
            .ToListAsync();

        if (attachments.Count == 0)
            return NotFound("No task attachments to back up.");

        // Serialize to JSON
        var json = JsonSerializer.Serialize(attachments, new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        });

        var bytes = Encoding.UTF8.GetBytes(json);
        using var stream = new MemoryStream(bytes);

        // Build blob path
        var now = DateTime.UtcNow;
        var blobPath = $"backups/{now:yyyy}/{now:MM}/{now:dd}/task-attachments-{now:HH-mm-ss}.json";

        var connectionString = _configuration["AzureStorage"];
        var containerClient = new BlobContainerClient(connectionString, "backups");

        await containerClient.CreateIfNotExistsAsync();
        await containerClient.UploadBlobAsync(blobPath, stream);

        return Ok(new
        {
            message = "Backup successful.",
            recordsBackedUp = attachments.Count,
            blobPath
        });
    }
}
