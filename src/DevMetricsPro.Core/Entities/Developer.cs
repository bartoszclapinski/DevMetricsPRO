namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Represents a developer in the system
/// </summary>
public class Developer
{
    /// <summary>
    /// Unique identifier for the developer
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Developer's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Developer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Developer's GitHub username (optional)
    /// </summary>
    public string? GitHubUsername { get; set; }

    /// <summary>
    /// When this developer record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this developer record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

