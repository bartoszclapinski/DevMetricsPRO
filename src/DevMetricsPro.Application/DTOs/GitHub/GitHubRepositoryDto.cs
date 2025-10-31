namespace DevMetricsPro.Application.DTOs.GitHub;

/// <summary>
/// DTO for GitHub repository information
/// </summary>
public class GitHubRepositoryDto
{
    /// <summary>
    /// GitHub repository ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Repository name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Repository description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Repository HTML URL
    /// </summary>
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>
    /// Whether the repository is private
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Whether the repository is a fork
    /// </summary>
    public bool IsFork { get; set; }

    /// <summary>
    /// Number of stars
    /// </summary>
    public int StargazersCount { get; set; }

    /// <summary>
    /// Number of forks
    /// </summary>
    public int ForksCount { get; set; }

    /// <summary>
    /// Number of open issues
    /// </summary>
    public int OpenIssuesCount { get; set; }

    /// <summary>
    /// Primary programming language
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Repository creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Last push timestamp
    /// </summary>
    public DateTime? PushedAt { get; set; }
}