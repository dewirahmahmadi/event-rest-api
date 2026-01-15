using System.ComponentModel.DataAnnotations;
using EventTicketing.Domain.Enums;

namespace EventTicketing.Application.DTOs.Users;

public class UpdateUserDTO
{
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }
}

public class UpdateUserPasswordDTO
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
