namespace TaskManagement.API.DTOs.TaskDtos;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; } // Open, InProgress, Completed
    public DateTime? DueDate { get; set; }

    public IFormFile? Attachment { get; set; } // Optional new file
}