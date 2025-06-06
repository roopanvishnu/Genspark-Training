using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Request.Task;

public class AssignTaskRequestDto
{
    [Required(ErrorMessage = "UserId is Required")]
    public Guid UserId{ get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}