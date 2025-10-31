using DevMetricsPro.Core.Enums;

namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Represents a source code repository (Github, GitLab, Azure)
/// </summary>
public class Repository : BaseEntity
{
    /// <summary>
    /// Repository name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Repository description (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Platform where the repository is hosted (GitHub, GitLab, Azure)
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// External ID from the platform
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Full URL of the repository
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Default branch name (default: main)
    /// </summary>
    public string? DefaultBranch { get; set; } = "main";

    /// <summary>
    /// Whether the repository is actively being tracked (default: true)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last time we synchronized data from this repository
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }
    
    /// <summary>
    /// Full repository name in owner/repo format (GitHub specific)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Whether the repository is private
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Whether the repository is a fork
    /// </summary>
    public bool IsFork { get; set; }

    /// <summary>
    /// Number of stars (GitHub/GitLab)
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
    /// Last push timestamp (from platform)
    /// </summary>
    public DateTime? PushedAt { get; set; }


    // Navigation properties - relationships to other entities

    /// <summary>
    /// Developers who contribute to this repository
    /// </summary>
    public ICollection<Developer> Contributors { get; set; } = new List<Developer>();

    /// <summary>
    /// All commits in this repository
    /// </summary>
    public ICollection<Commit> Commits { get; set; } = new List<Commit>();

    /// <summary>
    /// All pull requests in this repository
    /// </summary>
    public ICollection<PullRequest> PullRequests { get; set; } = new List<PullRequest>();

}