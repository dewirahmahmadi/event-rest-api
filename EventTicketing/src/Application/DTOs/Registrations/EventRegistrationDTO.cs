using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.DTOs;

public class EventRegistrationDTO
{
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventStartDate { get; set; }
    public DateTime EventEndDate { get; set; }
    public string EventLocation { get; set; } = string.Empty;
    
    public EventRegistrationDTO() { }

    public EventRegistrationDTO(Event entity)
    {
        this.EventId = entity.Id;
        this.EventTitle = entity.Title;
        this.EventStartDate = entity.StartDate;
        this.EventEndDate = entity.EndDate;
        this.EventLocation = entity.Location;
    }
    
}