namespace TaskManagement.API.Models;

public class TaskAttachment
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}
