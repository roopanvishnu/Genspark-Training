namespace TaskManagement.API.DTOs.TaskDtos;

public class TeamTaskUpdateDto
{
    public string Status { get; set; } = default!;
    public string? Comment { get; set; } // optional
}
