using EventTicketing.Domain.Entities;
using EventTicketing.Infrastructure.Data;
using EventTicketing.Application.DTOs.Events;
using EventTicketing.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Application.Services;

public class EventService
{
    private readonly DataDbContext _dbContext;

    public EventService(DataDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EventResponseDTO?> GetEventByIdAsync(Guid id)
    {
        var eventEntity = await _dbContext.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventEntity == null)
            return null;

        return await MapToEventResponseDTOAsync(eventEntity);
    }

    public async Task<EventResponseDTO> CreateEventAsync(CreateEventDTO createDto)
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Description = createDto.Description,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Location = createDto.Location,
            MaxAttendees = createDto.MaxAttendees,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Events.Add(eventEntity);
        await _dbContext.SaveChangesAsync();

        return await MapToEventResponseDTOAsync(eventEntity);
    }

    public async Task<EventResponseDTO?> UpdateEventAsync(Guid id, UpdateEventDTO updateDto)
    {
        var existingEvent = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent == null)
            return null;

        existingEvent.Title = updateDto.Title;
        existingEvent.Description = updateDto.Description;
        existingEvent.StartDate = updateDto.StartDate;
        existingEvent.EndDate = updateDto.EndDate;
        existingEvent.Location = updateDto.Location;
        existingEvent.MaxAttendees = updateDto.MaxAttendees;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await MapToEventResponseDTOAsync(existingEvent);
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var eventEntity = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventEntity != null)
        {
            _dbContext.Events.Remove(eventEntity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResponse<EventResponseDTO>> GetAllEventsAsync(PaginationRequest pagination)
    {
        var totalCount = await _dbContext.Events.CountAsync();

        var events = await _dbContext.Events
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt)
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        var eventDtos = new List<EventResponseDTO>();
        foreach (var eventEntity in events)
        {
            var dto = await MapToEventResponseDTOAsync(eventEntity);
            eventDtos.Add(dto);
        }

        return new PaginatedResponse<EventResponseDTO>
        {
            Data = eventDtos,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<int> GetEventRegistrationCountAsync(Guid eventId)
    {
        return await _dbContext.EventRegistrations
            .Where(er => er.EventId == eventId && er.IsAttending)
            .CountAsync();
    }

    private async Task<EventResponseDTO> MapToEventResponseDTOAsync(Event eventEntity)
    {
        var registrationCount = await GetEventRegistrationCountAsync(eventEntity.Id);

        return new EventResponseDTO
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location,
            MaxAttendees = eventEntity.MaxAttendees,
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt ?? eventEntity.CreatedAt,
            CurrentRegistrations = registrationCount,
            IsFull = registrationCount >= eventEntity.MaxAttendees
        };
    }
}
