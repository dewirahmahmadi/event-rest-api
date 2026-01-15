using EventTicketing.Domain.Enums;

namespace EventTicketing.Domain.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Guest;
    public bool IsActive { get; set; } = true;

    public Profile? Profile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
}
