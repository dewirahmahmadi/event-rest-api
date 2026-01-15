using EventTicketing.Domain.Enums;

namespace EventTicketing.Domain.Entities;

public class EventRegistration : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    // Live attendance tracking
    public bool IsAttending { get; set; } = false;
    public DateTime? CheckedInAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }
    
    public User User { get; set; } = null!;
    public Event Event { get; set; } = null!;
}