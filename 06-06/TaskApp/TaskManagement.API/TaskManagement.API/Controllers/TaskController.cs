using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.TaskDtos;
using TaskManagement.API.Filters;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/v1/tasks")]
[ServiceFilter(typeof(CustomExceptionFilter))]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    // POST /api/v1/tasks
    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromForm] CreateTaskDto dto)
    {
        _logger.LogInformation("Attempting to create new task");
        
        var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _taskService.CreateTaskAsync(dto, createdBy!);

        if (result == null)
        {
            _logger.LogWarning("Task creation failed - invalid assignee or task creation failed by user: {UserId}", createdBy);
            return BadRequest(new { success = false, message = "Invalid assignee or task creation failed" });
        }

        _logger.LogInformation("Successfully created task with ID: {TaskId} by user: {UserId}", result.Id, createdBy);
        return Ok(new
        {
            success = true,
            message = "Task created successfully",
            data = result
        });
    }

    // POST /api/v1/tasks/{id}/assign/{userId}
    [Authorize(Roles = "Manager")]
    [HttpPost("{id}/assign/{userId}")]
    public async Task<IActionResult> AssignToUser(Guid id, Guid userId)
    {
        _logger.LogInformation("Attempting to assign task ID: {TaskId} to user ID: {UserId}", id, userId);
        
        var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _taskService.AssignTaskToUserAsync(id, userId, managerId);
        if (!result)
        {
            _logger.LogWarning("Task assignment failed for task ID: {TaskId} to user ID: {UserId} by manager: {ManagerId}", id, userId, managerId);
            return BadRequest(new { success = false, message = "Assignment failed. Check task or user ID." });
        }

        _logger.LogInformation("Successfully assigned task ID: {TaskId} to user ID: {UserId} by manager: {ManagerId}", id, userId, managerId);
        return Ok(new { success = true, message = "Task assigned to user successfully" });
    }

    // POST /api/v1/tasks/{id}/broadcast
    [Authorize(Roles = "Manager")]
    [HttpPost("{id}/broadcast")]
    public async Task<IActionResult> BroadcastToAll(Guid id)
    {
        _logger.LogInformation("Attempting to broadcast task ID: {TaskId} to all team members", id);
        
        var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var count = await _taskService.BroadcastTaskToAllAsync(id, managerId);
        if (count == 0)
        {
            _logger.LogWarning("Task broadcast failed for task ID: {TaskId} by manager: {ManagerId}", id, managerId);
            return BadRequest(new { success = false, message = "Broadcast failed. Invalid task or no team members." });
        }

        _logger.LogInformation("Successfully broadcasted task ID: {TaskId} to {Count} team members by manager: {ManagerId}", id, count, managerId);
        return Ok(new
        {
            success = true,
            message = $"Task broadcasted to {count} team members"
        });
    }

    // GET /api/v1/tasks/assigned
    [Authorize(Roles = "TeamMember")]
    [HttpGet("assigned")]
    public async Task<IActionResult> GetAssignedTasks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching assigned tasks for user ID: {UserId}", userId);
        
        var result = await _taskService.GetAssignedTasksAsync(userId!);

        _logger.LogInformation("Successfully fetched {TaskCount} assigned tasks for user ID: {UserId}", result?.Count() ?? 0, userId);
        return Ok(new
        {
            success = true,
            message = "Assigned tasks fetched",
            data = result
        });
    }

    // GET /api/v1/tasks/{id}/download
    [Authorize]
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadTaskFile(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        
        _logger.LogInformation("Attempting to download attachment for task ID: {TaskId} by user: {UserId} with role: {Role}", id, userId, role);

        var fileResult = await _taskService.DownloadAttachmentAsync(id, userId, role);
        if (fileResult == null)
        {
            _logger.LogWarning("File download failed - not found or access denied for task ID: {TaskId} by user: {UserId}", id, userId);
            return NotFound(new { success = false, message = "File not found or access denied" });
        }

        _logger.LogInformation("Successfully downloaded attachment for task ID: {TaskId} by user: {UserId}", id, userId);
        return fileResult;
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromForm] UpdateTaskDto dto)
    {
        _logger.LogInformation("Attempting to update task ID: {TaskId}", id);
        
        var updatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _taskService.UpdateTaskAsync(id, dto, updatedBy);
        if (!result)
        {
            _logger.LogWarning("Task update failed for task ID: {TaskId} by user: {UserId}", id, updatedBy);
            return NotFound(new { success = false, message = "Task not found or update failed" });
        }

        _logger.LogInformation("Successfully updated task ID: {TaskId} by user: {UserId}", id, updatedBy);
        return Ok(new { success = true, message = "Task updated successfully" });
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        
        _logger.LogInformation("Fetching task ID: {TaskId} by user: {UserId} with role: {Role}", id, userId, role);

        var task = await _taskService.GetTaskByIdAsync(id, userId, role);
        if (task == null)
        {
            _logger.LogWarning("Task not found or access denied for task ID: {TaskId} by user: {UserId}", id, userId);
            return NotFound(new { success = false, message = "Task not found or access denied" });
        }

        _logger.LogInformation("Successfully fetched task ID: {TaskId} by user: {UserId}", id, userId);
        return Ok(new { success = true, message = "Task fetched", data = task });
    }

    [Authorize(Roles = "Manager")]
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] string? status, [FromQuery] Guid? assigneeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching tasks with filters - Status: {Status}, AssigneeId: {AssigneeId}, Page: {Page}, PageSize: {PageSize}", 
            status, assigneeId, page, pageSize);
        
        if (pageSize > 100) pageSize = 100;

        var (tasks, total) = await _taskService.GetTasksAsync(status, assigneeId, page, pageSize);

        _logger.LogInformation("Successfully fetched {TaskCount} tasks out of {TotalRecords} total records", tasks?.Count() ?? 0, total);
        return Ok(new
        {
            success = true,
            message = "Tasks fetched",
            data = tasks,
            pagination = new
            {
                page,
                pageSize,
                totalRecords = total,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            }
        });
    }

    [Authorize]
    [HttpGet("{id}/attachments")]
    public async Task<IActionResult> GetAttachments(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        
        _logger.LogInformation("Fetching attachments for task ID: {TaskId} by user: {UserId} with role: {Role}", id, userId, role);

        var attachments = await _taskService.GetAttachmentsAsync(id, userId, role);
        if (attachments == null)
        {
            _logger.LogWarning("Attachments not found or access denied for task ID: {TaskId} by user: {UserId}", id, userId);
            return NotFound(new { success = false, message = "Task not found or access denied" });
        }

        _logger.LogInformation("Successfully fetched {AttachmentCount} attachments for task ID: {TaskId} by user: {UserId}", 
            attachments?.Count() ?? 0, id, userId);
        return Ok(new { success = true, message = "Attachments fetched", data = attachments });
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("attachments/{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAttachment(Guid id, IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _logger.LogInformation("Attempting to upload attachment for task ID: {TaskId} by user: {UserId}, File: {FileName}", 
            id, userId, file?.FileName);

        var result = await _taskService.UploadAttachmentAsync(id, file, userId);

        if (!result)
        {
            _logger.LogWarning("Attachment upload failed for task ID: {TaskId} by user: {UserId}", id, userId);
            return BadRequest(new { success = false, message = "Upload failed or unauthorized" });
        }

        _logger.LogInformation("Successfully uploaded attachment for task ID: {TaskId} by user: {UserId}, File: {FileName}", 
            id, userId, file?.FileName);
        return Ok(new { success = true, message = "Attachment uploaded successfully" });
    }

    [Authorize]
    [HttpGet("/api/v1/files/{filename}")]
    public async Task<IActionResult> DownloadByFilename(string filename)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Attempting to download file by filename: {FileName} by user: {UserId}", filename, userId);

        var result = await _taskService.DownloadFileByNameAsync(filename);
        if (result == null)
        {
            _logger.LogWarning("File not found for filename: {FileName} by user: {UserId}", filename, userId);
            return NotFound(new { success = false, message = "File not found" });
        }

        _logger.LogInformation("Successfully downloaded file: {FileName} by user: {UserId}", filename, userId);
        return result;
    }
}