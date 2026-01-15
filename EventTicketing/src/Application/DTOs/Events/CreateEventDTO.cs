using System.ComponentModel.DataAnnotations;

namespace EventTicketing.Application.DTOs.Events;

public class CreateEventDTO
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Location { get; set; } = string.Empty;
    
    [Range(1, 100000)]
    public int MaxAttendees { get; set; }
}