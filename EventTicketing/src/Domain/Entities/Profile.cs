using EventTicketing.Domain.Enums;

namespace EventTicketing.Domain.Entities;

public class Profile : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public User User { get; set; } = null!;
}