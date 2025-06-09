namespace TaskManagement.API.DTOs.TaskDtos;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid? AssigneeId { get; set; } // assign to one, or broadcast to all manually
    public IFormFile? Attachment { get; set; }
}