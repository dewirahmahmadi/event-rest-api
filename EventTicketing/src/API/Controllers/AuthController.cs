using EventTicketing.Application.DTOs.Auth;
using EventTicketing.Application.Services;
using EventTicketing.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationDto = EventTicketing.Application.DTOs;

namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ApiControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<AuthResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .Select(x => new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = x.Value!.Errors.First().ErrorMessage,
                    Field = x.Key
                })
                .ToList();

            return BadRequest("Validation failed", errors);
        }

        var result = await _authService.SignUpAsync(signUpDto);

        if (result == null)
            return Conflict("User with this email already exists");

        return Created(result, "User registered successfully");
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .Select(x => new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = x.Value!.Errors.First().ErrorMessage,
                    Field = x.Key
                })
                .ToList();

            return BadRequest("Validation failed", errors);
        }

        var result = await _authService.LoginAsync(loginDto);

        if (result == null)
            return Unauthorized("Invalid email or password");

        return Success(result, "Login successful");
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .Select(x => new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = x.Value!.Errors.First().ErrorMessage,
                    Field = x.Key
                })
                .ToList();

            return BadRequest("Validation failed", errors);
        }

        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

        if (result == null)
            return Unauthorized("Invalid or expired refresh token");

        return Success(result, "Token refreshed successfully");
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDTO refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .Select(x => new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = x.Value!.Errors.First().ErrorMessage,
                    Field = x.Key
                })
                .ToList();

            return BadRequest("Validation failed", errors);
        }

        await _authService.RevokeTokenAsync(refreshTokenDto.RefreshToken);

        return Success<object>(null!, "Logout successful");
    }
}
