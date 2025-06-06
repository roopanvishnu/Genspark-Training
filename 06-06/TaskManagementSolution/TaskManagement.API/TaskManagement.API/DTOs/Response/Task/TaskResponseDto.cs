using TaskManagement.API.Models.Enums;
using TaskStatus = TaskManagement.API.Models.Enums.TaskStatus;

namespace TaskManagement.API.DTOs.Response.Task;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<TaskAssignmentResponseDto> Assignments { get; set; } = new List<TaskAssignmentResponseDto>();
    public List<FileResponseDto> Attachments { get; set; } = new List<FileResponseDto>();
}
public class TaskAssignmentResponseDto
{
    public Guid Id { get; set; }
    public Guid AssignedToUserId { get; set; }
    public string AssignedToUserName { get; set; } = string.Empty;
    public Guid AssignedByUserId { get; set; }
    public string AssignedByUserName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}