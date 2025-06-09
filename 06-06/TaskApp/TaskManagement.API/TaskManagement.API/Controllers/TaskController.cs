using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.TaskDtos;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers;

    [ApiController]
    [Route("api/v1/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // POST /api/v1/tasks
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromForm] CreateTaskDto dto)
        {
            var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _taskService.CreateTaskAsync(dto, createdBy!);

            if (result == null)
                return BadRequest(new { success = false, message = "Invalid assignee or task creation failed" });

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
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _taskService.AssignTaskToUserAsync(id, userId, managerId);
            if (!result)
                return BadRequest(new { success = false, message = "Assignment failed. Check task or user ID." });

            return Ok(new { success = true, message = "Task assigned to user successfully" });
        }

        // POST /api/v1/tasks/{id}/broadcast
        [Authorize(Roles = "Manager")]
        [HttpPost("{id}/broadcast")]
        public async Task<IActionResult> BroadcastToAll(Guid id)
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var count = await _taskService.BroadcastTaskToAllAsync(id, managerId);
            if (count == 0)
                return BadRequest(new { success = false, message = "Broadcast failed. Invalid task or no team members." });

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
            var result = await _taskService.GetAssignedTasksAsync(userId!);

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

            var fileResult = await _taskService.DownloadAttachmentAsync(id, userId, role);
            if (fileResult == null)
                return NotFound(new { success = false, message = "File not found or access denied" });

            return fileResult;
        }
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromForm] UpdateTaskDto dto)
        {
            var updatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _taskService.UpdateTaskAsync(id, dto, updatedBy);
            if (!result)
                return NotFound(new { success = false, message = "Task not found or update failed" });

            return Ok(new { success = true, message = "Task updated successfully" });
        }

    }

