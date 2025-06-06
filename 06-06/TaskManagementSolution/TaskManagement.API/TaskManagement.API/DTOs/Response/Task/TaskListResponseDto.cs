using TaskManagement.API.Models.Enums;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace TaskManagement.API.DTOs.Response.Task;

public class TaskListResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int AssignmentCount { get; set; }
    public int AttachmentCount { get; set; }
    public List<string> AssignedUserNames { get; set; } = new List<string>();
}