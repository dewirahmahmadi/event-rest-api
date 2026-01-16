using Microsoft.AspNetCore.SignalR;

namespace EventTicketing.Hub;

public class EventHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly EventConnectionTracker _connectionTracker;
    private readonly ILogger<EventHub> _logger;

    public EventHub(EventConnectionTracker connectionTracker, ILogger<EventHub> logger)
    {
        _connectionTracker = connectionTracker;
        _logger = logger;
    }

    public async Task JoinEvent(string eventId, string userId)
    {
        _logger.LogInformation("User {UserId} joining event {EventId} with connection {ConnectionId}", 
            userId, eventId, Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"event-{eventId}");
        _connectionTracker.AddConnection(eventId, Context.ConnectionId);
        
        var currentViewers = _connectionTracker.GetConnectionCount(eventId);
        await Clients.Group($"event-{eventId}").SendAsync("UserJoined", new
        {
            EventId = eventId,
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            CurrentViewers = currentViewers,
            Timestamp = DateTime.UtcNow
        });

        _logger.LogInformation("UserJoined event sent to group event-{EventId}", eventId);
    }

    public async Task LeaveEvent(string eventId, string userId)
    {
        _logger.LogInformation("User {UserId} leaving event {EventId} with connection {ConnectionId}", 
            userId, eventId, Context.ConnectionId);
        
        var currentViewersBefore = _connectionTracker.GetConnectionCount(eventId);
        var currentViewersAfter = currentViewersBefore - 1;
        
        await Clients.Group($"event-{eventId}").SendAsync("UserLeft", new
        {
            EventId = eventId,
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            CurrentViewers = currentViewersAfter,
            Timestamp = DateTime.UtcNow
        });
        
        _connectionTracker.RemoveConnection(eventId, Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event-{eventId}");

        _logger.LogInformation("User {UserId} removed from event {EventId}", userId, eventId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}, Exception: {Exception}", 
            Context.ConnectionId, exception?.Message);

        var affectedEvents = _connectionTracker.RemoveConnectionFromAllEvents(Context.ConnectionId);
        
        foreach (var (eventId, currentViewers) in affectedEvents)
        {
            await Clients.Group($"event-{eventId}").SendAsync("UserLeft", new
            {
                EventId = eventId,
                ConnectionId = Context.ConnectionId,
                CurrentViewers = currentViewers,
                Reason = "Disconnected",
                Timestamp = DateTime.UtcNow
            });
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
