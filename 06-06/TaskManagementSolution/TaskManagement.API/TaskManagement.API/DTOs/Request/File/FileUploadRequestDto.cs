using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Request.File;

public class FileUploadRequestDto
{
    // handling file input
    [Required(ErrorMessage = "File is Required")]
    public IFormFile File{ get; set; } = null!;
    
    // task id validation
    [Required(ErrorMessage = "Task Id is Required")]
    public Guid TaskId { get; set; }
}