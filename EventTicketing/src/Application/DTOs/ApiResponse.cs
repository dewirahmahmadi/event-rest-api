namespace EventTicketing.Application.DTOs;

public enum ResponseStatus
{
    Success,
    Created,
    BadRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    UnprocessableEntity,
    InternalServerError,
    ServiceUnavailable
}

public static class ResponseStatusExtensions
{
    public static int ToHttpStatusCode(this ResponseStatus status) => status switch
    {
        ResponseStatus.Success => StatusCodes.Status200OK,
        ResponseStatus.Created => StatusCodes.Status201Created,
        ResponseStatus.BadRequest => StatusCodes.Status400BadRequest,
        ResponseStatus.Unauthorized => StatusCodes.Status401Unauthorized,
        ResponseStatus.Forbidden => StatusCodes.Status403Forbidden,
        ResponseStatus.NotFound => StatusCodes.Status404NotFound,
        ResponseStatus.Conflict => StatusCodes.Status409Conflict,
        ResponseStatus.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
        ResponseStatus.InternalServerError => StatusCodes.Status500InternalServerError,
        ResponseStatus.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status500InternalServerError
    };
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Results { get; set; }
    public ResponseStatus Status { get; set; }
    public List<ApiError> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Path { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Request successful")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Results = data,
            Status = ResponseStatus.Success
        };
    }

    public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Results = data,
            Status = ResponseStatus.Created
        };
    }

    public static ApiResponse<T> BadRequest(string message = "Bad request", List<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.BadRequest,
            Errors = errors ?? new List<ApiError>()
        };
    }

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.Unauthorized
        };
    }

    public static ApiResponse<T> Forbidden(string message = "Access forbidden")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.Forbidden
        };
    }

    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.NotFound
        };
    }

    public static ApiResponse<T> Conflict(string message = "Resource conflict")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.Conflict
        };
    }

    public static ApiResponse<T> UnprocessableEntity(string message = "Unprocessable entity", List<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.UnprocessableEntity,
            Errors = errors ?? new List<ApiError>()
        };
    }

    public static ApiResponse<T> InternalServerError(string message = "Internal server error")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.InternalServerError
        };
    }

    public static ApiResponse<T> ServiceUnavailable(string message = "Service unavailable")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Status = ResponseStatus.ServiceUnavailable
        };
    }
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Field { get; set; }
}