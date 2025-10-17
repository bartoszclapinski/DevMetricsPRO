namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Represents a developer in the system
/// </summary>
public class Developer : BaseEntity
{
    /// <summary>
    /// Developer's email address (required, unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Developer's GitHub username (optional)
    /// </summary>
    public string? GitHubUsername { get; set; }
    
    /// <summary>
    /// Developer's GitLab username (optional)
    /// </summary>
    public string? GitLabUsername { get; set; }

    /// <summary>
    /// Developer's avatar URL (optional)
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Developer's display name (optional)
    /// </summary>
    public string? DisplayName { get; set; }

    // Navigation properties - relationships to other entities

    /// <summary>
    /// Repositories the developer has contributed to
    /// </summary>
    public ICollection<Repository> Repositories { get; set; } = new List<Repository>();

    /// <summary>
    /// Commits the developer has made
    /// </summary>
    public ICollection<Commit> Commits { get; set; } = new List<Commit>();

    /// <summary>
    /// Metrics calculated for the developer
    /// </summary>
    public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
  
}

