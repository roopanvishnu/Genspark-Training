using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.TaskDtos;

namespace TaskManagement.API.Interfaces;

public interface ITaskService
{
    Task<TaskDto?> CreateTaskAsync(CreateTaskDto dto, string createdBy);
    Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId, string assignerId);
    Task<int> BroadcastTaskToAllAsync(Guid taskId, string assignerId);
    Task<FileStreamResult?> DownloadAttachmentAsync(Guid taskId, string currentUserId, string role);
    Task<List<TaskDto>> GetAssignedTasksAsync(string currentUserId);
    Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto, string updatedBy);
    Task<TaskDto?> GetTaskByIdAsync(Guid id, string userId, string role);
    Task<(List<TaskDto> tasks, int total)> GetTasksAsync(string? status, Guid? assigneeId, int page, int pageSize);
    Task<List<string>?> GetAttachmentsAsync(Guid taskId, string userId, string role);
    Task<bool> UploadAttachmentAsync(Guid taskId, IFormFile file, string userId);
    Task<FileStreamResult?> DownloadFileByNameAsync(string filename);
}