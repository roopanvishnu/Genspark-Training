using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.DTOs.Request.Task;

public class CreateTaskRequestDto
{
    // Title validation
    [Required(ErrorMessage = "Title is Required")]
    [StringLength(200, ErrorMessage = "Title Cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    // Description Validation
    [StringLength(1000,ErrorMessage = "Description Cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    // Task Priority
    public TaskPriority? Priority { get; set; } = TaskPriority.Medium;
    
    // DueDate
    public DateTime? DueDate { get; set; }
}