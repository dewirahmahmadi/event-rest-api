using EventTicketing.Application.DTOs;
using System.Net;
using System.Text.Json;

namespace EventTicketing.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ApiResponse<object>
        {
            Success = false,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path
        };

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
                errorResponse.Status = ResponseStatus.BadRequest;
                errorResponse.Message = "Invalid request parameters";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                errorResponse.Status = ResponseStatus.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                errorResponse.Status = ResponseStatus.NotFound;
                errorResponse.Message = "Resource not found";
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidOperationException:
                errorResponse.Status = ResponseStatus.UnprocessableEntity;
                errorResponse.Message = "Invalid operation";
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                break;

            default:
                errorResponse.Status = ResponseStatus.InternalServerError;
                errorResponse.Message = _env.IsDevelopment() ? exception.Message : "An internal server error occurred";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        if (_env.IsDevelopment())
        {
            errorResponse.Errors.Add(new ApiError
            {
                Code = "EXCEPTION",
                Message = exception.Message,
                Field = exception.StackTrace?.Split('\n')[0].Trim()
            });
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(json);
    }
}