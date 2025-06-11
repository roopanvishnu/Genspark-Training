namespace TaskManagement.API.DTOs.Response;

public class ApiErrorDto
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}