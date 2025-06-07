using TaskManagement.API.DTOs.Pagination;
using TaskManagement.API.DTOs.Request.Task;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.DTOs.Response.Task;
using TaskManagement.API.Models.Entities;
using TaskManagement.API.Repositories.Interfaces;
using TaskManagement.API.Repositories.Interfaces.UnitOfWork;

namespace TaskManagement.API.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public TaskService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<PaginatedResponseDto<TaskListResponseDto>> GetTasksAsync(TaskFilterRequestDto filter, PaginationParameters pagination)
    {
        var paged = await _unitOfWork.Tasks.GetFilteredTasksAsync(filter, pagination);
        var data = paged.Items.Select(t => new TaskListResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            DueDate = t.DueDate,
            CreatedByName = t.CreatedBy.FirstName + " " + t.CreatedBy.LastName,
            CreatedAt = t.CreatedAt,
            AssignmentCount = t.TaskAssignments.Count,
            AttachmentCount = t.FileAttachments.Count,
            AssignedUserNames = t.TaskAssignments.Where(a => a.IsActive).Select(a => a.AssignedToUser.FirstName + " " + a.AssignedToUser.LastName).ToList()
        }).ToList();

        return new PaginatedResponseDto<TaskListResponseDto>
        {
            Success = true,
            Message = "Tasks fetched",
            Data = data,
            Pagination = new PaginationMetadata
            {
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalPages = paged.TotalPages
            }
        };
    }

    public async Task<ApiResponseDto<TaskResponseDto>> GetTaskByIdAsync(Guid taskId)
    {
        var task = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(taskId);
        if (task == null)
            return ApiResponseDto<TaskResponseDto>.ErrorResponse("Task not found");

        return ApiResponseDto<TaskResponseDto>.SuccessResponse(MapToDto(task));
    }

    public async Task<ApiResponseDto<TaskResponseDto>> CreateTaskAsync(CreateTaskRequestDto request, string createdBy)
    {
        var userId = Guid.Parse(createdBy);
        var task = new Task
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CreatedById = userId,
            CreatedBy = await _unitOfWork.Users.GetByIdAsync(userId) ?? new User()
        };

        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        await _notificationService.NotifyTaskCreatedAsync(task.Id, task.Title, createdBy);

        return ApiResponseDto<TaskResponseDto>.SuccessResponse(MapToDto(task));
    }

    public async Task<ApiResponseDto<TaskResponseDto>> UpdateTaskAsync(Guid taskId, UpdateTaskRequestDto request, string updatedBy)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        if (task == null)
            return ApiResponseDto<TaskResponseDto>.ErrorResponse("Task not found");

        var oldStatus = task.Status.ToString();
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Tasks.UpdateAsync(task);
        await _unitOfWork.SaveChangesAsync();

        if (oldStatus != task.Status.ToString())
        {
            await _notificationService.NotifyTaskStatusChangedAsync(task.Id, task.Title, oldStatus, task.Status.ToString(), updatedBy);
        }
        else
        {
            await _notificationService.NotifyTaskUpdatedAsync(task.Id, task.Title, updatedBy);
        }

        return ApiResponseDto<TaskResponseDto>.SuccessResponse(MapToDto(task));
    }

    public async Task<ApiResponseDto<string>> DeleteTaskAsync(Guid taskId, string deletedBy)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        if (task == null)
            return ApiResponseDto<string>.ErrorResponse("Task not found");

        await _unitOfWork.Tasks.DeleteAsync(task);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponseDto<string>.SuccessResponse("Task deleted");
    }

    public async Task<ApiResponseDto<TaskResponseDto>> AssignTaskAsync(Guid taskId, AssignTaskRequestDto request, string assignedBy)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        if (task == null)
            return ApiResponseDto<TaskResponseDto>.ErrorResponse("Task not found");

        var assignment = new TaskAssignment
        {
            TaskId = taskId,
            AssignedToUserId = request.UserId,
            AssignedByUserId = Guid.Parse(assignedBy)
        };

        await _unitOfWork.TaskAssignments.AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync();

        await _notificationService.NotifyTaskAssignedAsync(task.Id, task.Title, request.UserId, assignedBy);

        var updatedTask = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(taskId);
        return ApiResponseDto<TaskResponseDto>.SuccessResponse(MapToDto(updatedTask!));
    }

    public async Task<ApiResponseDto<List<TaskListResponseDto>>> GetAssignedTasksAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
            return ApiResponseDto<List<TaskListResponseDto>>.ErrorResponse("Invalid user id");

        var tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(id);
        var list = tasks.Select(t => new TaskListResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            DueDate = t.DueDate,
            CreatedByName = t.CreatedBy.FirstName + " " + t.CreatedBy.LastName,
            CreatedAt = t.CreatedAt,
            AssignmentCount = t.TaskAssignments.Count,
            AttachmentCount = t.FileAttachments.Count,
            AssignedUserNames = t.TaskAssignments.Where(a => a.IsActive).Select(a => a.AssignedToUser.FirstName + " " + a.AssignedToUser.LastName).ToList()
        }).ToList();
        return ApiResponseDto<List<TaskListResponseDto>>.SuccessResponse(list);
    }

    private static TaskResponseDto MapToDto(Task task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedById = task.CreatedById,
            CreatedByName = task.CreatedBy.FirstName + " " + task.CreatedBy.LastName,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            Assignments = task.TaskAssignments.Select(a => new TaskAssignmentResponseDto
            {
                Id = a.Id,
                AssignedToUserId = a.AssignedToUserId,
                AssignedToUserName = a.AssignedToUser.FirstName + " " + a.AssignedToUser.LastName,
                AssignedByUserId = a.AssignedByUserId,
                AssignedByUserName = a.AssignedByUser.FirstName + " " + a.AssignedByUser.LastName,
                AssignedAt = a.AssignedAt,
                IsActive = a.IsActive,
                Notes = a.Notes
            }).ToList(),
            Attachments = task.FileAttachments.Select(f => new FileResponseDto
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalFileName = f.OriginalFileName,
                ContentType = f.ContentType,
                FileSizeBytes = f.FileSizeBytes,
                FilePath = f.FilePath,
                UploadedAt = f.UploadedAt
            }).ToList()
        };
    }
}
