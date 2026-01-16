using EventTicketing.Application.DTOs;
using EventTicketing.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationDto = EventTicketing.Application.DTOs;

namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RegistrationController : ApiControllerBase
{
    private readonly RegistrationService _registrationService;

    public RegistrationController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<PaginatedResponse<RegistrationResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllRegistrationsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest { Page = page, PageSize = pageSize };
        var registrations = await _registrationService.GetAllRegistrationsAsync(pagination);
        return SuccessPaginated(registrations, "Registrations retrieved successfully");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<RegistrationResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRegistrationById(Guid id)
    {
        var registration = await _registrationService.GetRegistrationByIdAsync(id);
        
        if (registration is null)
            return NotFound($"Registration with ID {id} not found");
        
        return Success(registration, "Registration retrieved successfully");
    }

    [HttpGet("event/{eventId:guid}")]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<PaginatedResponse<RegistrationResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegistrationsByEventIdPaginated(Guid eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest { Page = page, PageSize = pageSize };
        var registrations = await _registrationService.GetRegistrationsByEventIdAsync(eventId, pagination);
        return SuccessPaginated(registrations, "Registrations retrieved successfully");
    }
    
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<RegistrationResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationDto.ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRegistration([FromBody] CreateRegistrationDTO createDto)
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

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user");
        }

        try
        {
            var createdRegistration = await _registrationService.CreateRegistrationAsync(createDto, userId);
            return Created(createdRegistration, "Registration created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}