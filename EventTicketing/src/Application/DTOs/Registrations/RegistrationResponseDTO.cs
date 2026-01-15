using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.DTOs;

public class RegistrationResponseDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public bool IsAttending { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }

    public RegistrationResponseDTO(){}

    public RegistrationResponseDTO(EventRegistration registration)
    {
        this.Id = registration.Id;
        this.UserId = registration.UserId;
        this.EventId = registration.EventId;
        this.UserName = registration.User?.Email?.Split('@')[0] ?? string.Empty;
        this.UserEmail = registration.User?.Email ?? string.Empty;
        this.FirstName = registration.User?.Profile?.FirstName ?? string.Empty;
        this.LastName = registration.User?.Profile?.LastName ?? string.Empty;
        this.RegisteredAt = registration.RegisteredAt;
        this.IsAttending = registration.IsAttending;
        this.CheckedInAt = registration.CheckedInAt;
        this.CheckedOutAt = registration.CheckedOutAt;
    }
}