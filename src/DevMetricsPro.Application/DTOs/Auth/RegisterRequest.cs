using System.ComponentModel.DataAnnotations;

namespace DevMetricsPro.Application.DTOs.Auth;

public class RegisterRequest
{
    /// <summary>
    /// User's email address (used as username)
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation (must much Password)
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Optional display name for the user
    /// </summary>
    [StringLength(100, ErrorMessage = "Dispal name cannot exeed 100 characters")]
    public string? DisplayName { get; set; }
}