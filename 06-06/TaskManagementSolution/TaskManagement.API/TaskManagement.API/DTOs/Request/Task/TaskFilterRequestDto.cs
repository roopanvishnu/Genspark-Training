
using TaskManagement.API.Models.Enums;
using TaskStatus = TaskManagement.API.Models.Enums.TaskStatus;

namespace TaskManagement.API.DTOs.Request.Task;

public class TaskFilterRequestDto
{
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
}