namespace EventTicketing.Domain.Entities;

public class Event : BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxAttendees { get; set; }
    
    public int CurrentAttendeeCount { get; set; } = 0;
    
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}
