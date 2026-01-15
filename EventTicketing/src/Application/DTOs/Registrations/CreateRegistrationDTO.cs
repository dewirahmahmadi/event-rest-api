using System.ComponentModel.DataAnnotations;

namespace EventTicketing.Application.DTOs;

public class CreateRegistrationDTO
{
    [Required]
    public Guid EventId { get; set; }
}