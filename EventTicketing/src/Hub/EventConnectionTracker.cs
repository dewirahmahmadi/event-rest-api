using System.Collections.Concurrent;

namespace EventTicketing.Hub;

public class EventConnectionTracker
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _eventConnections = new();

    public void AddConnection(string eventId, string connectionId)
    {
        var connections = _eventConnections.GetOrAdd(eventId, _ => new ConcurrentDictionary<string, byte>());
        connections.TryAdd(connectionId, 0);
    }

    public void RemoveConnection(string eventId, string connectionId)
    {
        if (_eventConnections.TryGetValue(eventId, out var connections))
        {
            connections.TryRemove(connectionId, out _);
            if (connections.IsEmpty)
            {
                _eventConnections.TryRemove(eventId, out _);
            }
        }
    }
    
    public Dictionary<string, int> RemoveConnectionFromAllEvents(string connectionId)
    {
        var affectedEvents = new Dictionary<string, int>();

        foreach (var kvp in _eventConnections)
        {
            if (kvp.Value.TryRemove(connectionId, out _))
            {
                affectedEvents[kvp.Key] = kvp.Value.Count;
            }

            if (kvp.Value.IsEmpty)
            {
                _eventConnections.TryRemove(kvp.Key, out _);
            }
        }

        return affectedEvents;
    }

    public int GetConnectionCount(string eventId)
    {
        if (_eventConnections.TryGetValue(eventId, out var connections))
        {
            return connections.Count;
        }
        return 0;
    }

    public IEnumerable<string> GetConnections(string eventId)
    {
        if (_eventConnections.TryGetValue(eventId, out var connections))
        {
            return connections.Keys.ToList();
        }
        return Enumerable.Empty<string>();
    }
}
