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

}