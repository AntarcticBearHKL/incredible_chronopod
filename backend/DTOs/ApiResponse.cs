namespace backend.DTOs;

/// <summary>
/// 通用API响应格式
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// 分页响应格式
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "获取成功";
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    
    public static PagedResponse<T> Create(List<T> data, int page, int pageSize, int total, string message = "获取成功")
    {
        return new PagedResponse<T>
        {
            Data = data,
            Page = page,
            PageSize = pageSize,
            Total = total,
            Message = message
        };
    }
}