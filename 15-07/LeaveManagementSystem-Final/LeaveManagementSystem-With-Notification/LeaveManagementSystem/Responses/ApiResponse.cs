namespace LeaveManagementSystem.Responses {
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ApiResponse<T> FailureResponse(string message = "Error", Dictionary<string, List<string>>? errors = null)
        {
            return new ApiResponse<T> { Success = false, Message = message, Data = default, Errors = errors };
        }
    }

}