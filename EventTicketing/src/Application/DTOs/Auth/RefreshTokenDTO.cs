using System.ComponentModel.DataAnnotations;

namespace EventTicketing.Application.DTOs.Auth;

public class RefreshTokenDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
