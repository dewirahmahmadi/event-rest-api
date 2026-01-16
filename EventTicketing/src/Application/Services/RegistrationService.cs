using EventTicketing.Application.DTOs;
using EventTicketing.Domain.Entities;
using EventTicketing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Application.Services;

public class RegistrationService
{
    private readonly DataDbContext _dbContext;

    public RegistrationService(DataDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<RegistrationResponseDTO>> GetAllRegistrationsAsync(PaginationRequest pagination)
    {
        var totalCount = await _dbContext.EventRegistrations.CountAsync();

        var registrations = await _dbContext.EventRegistrations
            .AsNoTracking()
            .Include(er => er.User)
                .ThenInclude(u => u.Profile)
            .Include(er => er.Event)
            .OrderByDescending(er => er.RegisteredAt)
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<RegistrationResponseDTO>
        {
            Data = registrations.Select(er => new RegistrationResponseDTO(er)).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedResponse<RegistrationResponseDTO>> GetRegistrationsByEventIdAsync(Guid eventId, PaginationRequest pagination)
    {
        var totalCount = await _dbContext.EventRegistrations
            .Where(er => er.EventId == eventId)
            .CountAsync();

        var registrations = await _dbContext.EventRegistrations
            .AsNoTracking()
            .Where(er => er.EventId == eventId)
            .Include(er => er.User)
                .ThenInclude(u => u.Profile)
            .Include(er => er.Event)
            .OrderByDescending(er => er.RegisteredAt)
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<RegistrationResponseDTO>
        {
            Data = registrations.Select(er => new RegistrationResponseDTO(er)).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<RegistrationResponseDTO?> GetRegistrationByIdAsync(Guid id)
    {
        var registration = await _dbContext.EventRegistrations
            .AsNoTracking()
            .Include(er => er.User)
                .ThenInclude(u => u.Profile)
            .Include(er => er.Event)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (registration == null)
            return null;

        return new RegistrationResponseDTO(registration);
    }

    public async Task<RegistrationResponseDTO> CreateRegistrationAsync(CreateRegistrationDTO createDto, Guid userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new ArgumentException("User not found");

        var eventRegistration = new EventRegistration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = createDto.EventId,
            RegisteredAt = DateTime.UtcNow,
            IsAttending = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.EventRegistrations.Add(eventRegistration);
        await _dbContext.SaveChangesAsync();

        // Reload with includes for proper DTO mapping
        var createdRegistration = await _dbContext.EventRegistrations
            .Include(er => er.User)
                .ThenInclude(u => u.Profile)
            .Include(er => er.Event)
            .FirstAsync(er => er.Id == eventRegistration.Id);

        return new RegistrationResponseDTO(createdRegistration);
    }
}
