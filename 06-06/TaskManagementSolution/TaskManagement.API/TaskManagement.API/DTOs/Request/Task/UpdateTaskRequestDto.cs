using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.DTOs.Request.Task;

public class UpdateTaskRequestDto
{
    // Title validation
    [Required(ErrorMessage = "Title is Required")]
    [StringLength(200, ErrorMessage = "Title Cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    // Description Validation
    [StringLength(1000,ErrorMessage = "Description Cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    // Task Status 
    [Required(ErrorMessage = "Status is Required")]
    public TaskManagement.API.Models.Enums.TaskStatus Status { get; set; }
    
    // Task Priority
    public TaskPriority? Priority { get; set; } 
    
    // DueDate
    public DateTime? DueDate { get; set; }
}