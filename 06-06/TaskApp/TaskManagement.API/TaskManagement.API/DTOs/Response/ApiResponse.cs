namespace TaskManagement.API.DTOs.Response;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ResultsCount { get; set; } = 0;
    public T? Data { get; set; }
    public ApiErrorDto? Errors { get; set; }
}