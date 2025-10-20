using Microsoft.AspNetCore.Identity;

namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Application user with Identity integration
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Link to Developer profile
    /// </summary>
    public Guid? DeveloperId { get; set; }

    /// <summary>
    /// Navigation property to Developer
    /// </summary>
    public Developer? Developer { get; set; }

    /// <summary>
    /// When the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last successful login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}