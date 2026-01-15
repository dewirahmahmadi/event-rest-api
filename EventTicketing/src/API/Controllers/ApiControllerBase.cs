using Microsoft.AspNetCore.Mvc;
using EventTicketing.Application.DTOs;
using ApplicationDto = EventTicketing.Application.DTOs;

namespace EventTicketing.API.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected IActionResult ApiResponse<T>(ApplicationDto.ApiResponse<T> response)
    {
        response.Path = Request.Path;
        return StatusCode(response.Status.ToHttpStatusCode(), response);
    }

    protected IActionResult ApiResponse(ApplicationDto.ApiResponse<object> response)
    {
        response.Path = Request.Path;
        return StatusCode(response.Status.ToHttpStatusCode(), response);
    }

    protected IActionResult Success<T>(T data, string message = "Request successful")
    {
        return ApiResponse(ApplicationDto.ApiResponse<T>.Ok(data, message));
    }

    protected IActionResult SuccessPaginated<T>(PaginatedResponse<T> data, string message = "Request successful")
    {
        return Success(data, message);
    }

    protected IActionResult Created<T>(T data, string message = "Resource created successfully")
    {
        return ApiResponse(ApplicationDto.ApiResponse<T>.Created(data, message));
    }

    protected IActionResult BadRequest(string message = "Bad request", List<ApiError>? errors = null)
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.BadRequest(message, errors));
    }

    protected IActionResult Unauthorized(string message = "Unauthorized access")
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.Unauthorized(message));
    }

    protected IActionResult Forbidden(string message = "Access forbidden")
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.Forbidden(message));
    }

    protected IActionResult NotFound(string message = "Resource not found")
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.NotFound(message));
    }

    protected IActionResult Conflict(string message = "Resource conflict")
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.Conflict(message));
    }

    protected IActionResult UnprocessableEntity(string message = "Unprocessable entity", List<ApiError>? errors = null)
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.UnprocessableEntity(message, errors));
    }

    protected IActionResult InternalServerError(string message = "Internal server error")
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.InternalServerError(message));
    }

    protected IActionResult ServiceUnavailable(string message = "Service unavailable")
    {
        return ApiResponse(ApplicationDto.ApiResponse<object>.ServiceUnavailable(message));
    }
}