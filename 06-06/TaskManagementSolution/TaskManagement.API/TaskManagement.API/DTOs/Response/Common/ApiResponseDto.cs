namespace TaskManagement.API.DTOs.Response.Common;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public Dictionary<string, List<string>>? Errors { get; set; }

    public static ApiResponseDto<T> SuccessResponse(T data, string message = "Sucessfull")
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }
    public static ApiResponseDto<T> ErrorResponse(string message, Dictionary<string, List<string>>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}