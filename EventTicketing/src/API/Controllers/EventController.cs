using EventTicketing.Application.Services;
using EventTicketing.Application.DTOs.Events;
using EventTicketing.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventController : ApiControllerBase
{
    private readonly EventService _eventService;

    public EventController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EventResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEventsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest { Page = page, PageSize = pageSize };
        var events = await _eventService.GetAllEventsAsync(pagination);
        return SuccessPaginated(events, "Events retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        var eventItem = await _eventService.GetEventByIdAsync(id);

        if (eventItem is null)
            return NotFound($"Event with ID {id} not found");

        return Success(eventItem, "Event retrieved successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO createDto)
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

        var createdEvent = await _eventService.CreateEventAsync(createDto);
        return Created(createdEvent, "Event created successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDTO updateDto)
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

        var updatedEvent = await _eventService.UpdateEventAsync(id, updateDto);

        if (updatedEvent is null)
            return NotFound($"Event with ID {id} not found");

        return Success(updatedEvent, "Event updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var existingEvent = await _eventService.GetEventByIdAsync(id);
        if (existingEvent is null)
            return NotFound($"Event with ID {id} not found");

        await _eventService.DeleteEventAsync(id);
        return Success<object>(null!, "Event deleted successfully");
    }
}