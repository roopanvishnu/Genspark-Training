using TaskManagement.API.DTOs.Pagination;
using TaskManagement.API.DTOs.Request.Task;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.DTOs.Response.Task;

namespace TaskManagement.API.Repositories.Interfaces;

public interface ITaskService
{
    Task<PaginatedResponseDto<TaskListResponseDto>> GetTasksAsync(TaskFilterRequestDto filter, PaginationParameters pagination);
    Task<ApiResponseDto<TaskResponseDto>> GetTaskByIdAsync(Guid taskId);
    Task<ApiResponseDto<TaskResponseDto>> CreateTaskAsync(CreateTaskRequestDto request, string createdBy);
    Task<ApiResponseDto<TaskResponseDto>> UpdateTaskAsync(Guid taskId, UpdateTaskRequestDto request, string updatedBy);
    Task<ApiResponseDto<string>> DeleteTaskAsync(Guid taskId, string deletedBy);
    Task<ApiResponseDto<TaskResponseDto>> AssignTaskAsync(Guid taskId, AssignTaskRequestDto request, string assignedBy);
    Task<ApiResponseDto<List<TaskListResponseDto>>> GetAssignedTasksAsync(string userId);
}