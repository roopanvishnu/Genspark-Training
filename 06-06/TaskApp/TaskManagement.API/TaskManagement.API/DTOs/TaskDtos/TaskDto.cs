namespace TaskManagement.API.DTOs.TaskDtos;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "Open";
    public DateTime? DueDate { get; set; }

    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }

    public string? AttachmentFileName { get; set; }
}