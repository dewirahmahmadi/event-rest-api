
using Microsoft.AspNetCore.SignalR;

namespace EventTicketing.Hub;

public class EventHub : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task JoinEventGroup(string eventId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"event-{eventId}");
        await Clients.Caller.SendAsync("JoinedEvent", eventId);
        
        await Clients.Group($"event-{eventId}").SendAsync("UserJoined", new
        {
            ConnectionId = Context.ConnectionId,
            EventId = eventId,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public async Task LeaveEventGroup(string eventId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event-{eventId}");
        await Clients.Caller.SendAsync("LeftEvent", eventId);
        
        await Clients.Group($"event-{eventId}").SendAsync("UserLeft", new
        {
            ConnectionId = Context.ConnectionId,
            EventId = eventId,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        await Clients.Caller.SendAsync("JoinedUserGroup", userId);
    }
    
    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
        await Clients.Caller.SendAsync("LeftUserGroup", userId);
    }
    
    public async Task NotifyRegistrationCreated(string eventId, object registrationData)
    {
        await Clients.Group($"event-{eventId}").SendAsync("RegistrationCreated", new
        {
            EventId = eventId,
            Registration = registrationData,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public async Task NotifyRegistrationCancelled(string eventId, string registrationId)
    {
        await Clients.Group($"event-{eventId}").SendAsync("RegistrationCancelled", new
        {
            EventId = eventId,
            RegistrationId = registrationId,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public async Task NotifyCheckedIn(string eventId, string registrationId, string userName)
    {
        await Clients.Group($"event-{eventId}").SendAsync("AttendeeCheckedIn", new
        {
            EventId = eventId,
            RegistrationId = registrationId,
            UserName = userName,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public async Task NotifyEventCapacityUpdate(string eventId, int currentAttendees, int maxAttendees)
    {
        await Clients.Group($"event-{eventId}").SendAsync("CapacityUpdate", new
        {
            EventId = eventId,
            CurrentAttendees = currentAttendees,
            MaxAttendees = maxAttendees,
            AvailableSpots = maxAttendees - currentAttendees,
            IsFull = currentAttendees >= maxAttendees,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserDisconnected", new
        {
            ConnectionId = Context.ConnectionId,
            Timestamp = DateTime.UtcNow
        });
        
        await base.OnDisconnectedAsync(exception);
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", new
        {
            ConnectionId = Context.ConnectionId,
            Timestamp = DateTime.UtcNow,
            Message = "Successfully connected to EventHub"
        });
        
        await base.OnConnectedAsync();
    }
}