using Microsoft.AspNetCore.SignalR;
using EventTicketing.Hub;
using EventTicketing.Application.DTOs;

namespace EventTicketing.Application.Services;

public class EventNotificationService
{
    private readonly IHubContext<EventHub> _hubContext;

    public EventNotificationService(IHubContext<EventHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyRegistrationCreatedAsync(string eventId, RegistrationResponseDTO registration)
    {
        await _hubContext.Clients.Group($"event-{eventId}").SendAsync("RegistrationCreated", new
        {
            EventId = eventId,
            Registration = registration,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyRegistrationCancelledAsync(string eventId, string registrationId)
    {
        await _hubContext.Clients.Group($"event-{eventId}").SendAsync("RegistrationCancelled", new
        {
            EventId = eventId,
            RegistrationId = registrationId,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyAttendeeCheckedInAsync(string eventId, string registrationId, string userName)
    {
        await _hubContext.Clients.Group($"event-{eventId}").SendAsync("AttendeeCheckedIn", new
        {
            EventId = eventId,
            RegistrationId = registrationId,
            UserName = userName,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyCapacityUpdateAsync(string eventId, int currentAttendees, int maxAttendees)
    {
        await _hubContext.Clients.Group($"event-{eventId}").SendAsync("CapacityUpdate", new
        {
            EventId = eventId,
            CurrentAttendees = currentAttendees,
            MaxAttendees = maxAttendees,
            AvailableSpots = maxAttendees - currentAttendees,
            IsFull = currentAttendees >= maxAttendees,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyEventUpdatedAsync(string eventId, object eventData)
    {
        await _hubContext.Clients.Group($"event-{eventId}").SendAsync("EventUpdated", new
        {
            EventId = eventId,
            Event = eventData,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyEventDeletedAsync(string eventId)
    {
        await _hubContext.Clients.Group($"event-{eventId}").SendAsync("EventDeleted", new
        {
            EventId = eventId,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyUserRegistrationsAsync(string userId, object notification)
    {
        await _hubContext.Clients.Group($"user-{userId}").SendAsync("UserNotification", new
        {
            UserId = userId,
            Notification = notification,
            Timestamp = DateTime.UtcNow
        });
    }
}