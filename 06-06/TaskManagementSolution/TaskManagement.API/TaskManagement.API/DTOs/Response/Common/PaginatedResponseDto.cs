namespace TaskManagement.API.DTOs.Response.Common;

public class PaginatedResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<T> Data { get; set; } = new List<T>();
    public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    public Dictionary<string,List<string>>? Errors { get; set; }
}

public class PaginationMetadata
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}