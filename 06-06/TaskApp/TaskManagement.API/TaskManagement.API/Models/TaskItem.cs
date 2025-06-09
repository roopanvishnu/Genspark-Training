namespace TaskManagement.API.Models;
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;// Open, InProgress, Completed
    public DateTime? DueDate { get; set; }

    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;

    public ICollection<TaskAttachment> Attachments { get; set; } 
}
