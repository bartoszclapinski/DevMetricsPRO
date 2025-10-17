using DevMetricsPro.Core.Enums;

namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Represents a pull request or merge request in a source code repository
/// </summary>
public class PullRequest : BaseEntity
{
    /// <summary>
    /// Foreign key to the repository
    /// </summary>
    public Guid RepositoryId { get; set; }

    /// <summary>
    /// Foreign key to the developer who created the pull request
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// External ID from the platform (PR number from GitHub/GitLab)
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Pull request title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Pull request description (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current status of the pull request (Open, Closed, Merged, Draft)
    /// </summary>
    public PullRequestStatus Status { get; set; }

    /// <summary>
    /// When the pull request was merged (null if not merged)
    /// </summary>
    public DateTime? MergedAt { get; set; }

    /// <summary>
    /// When the pull request was closed (null if still open)
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Total number of comments on the pull request
    /// </summary>
    public int CommentsCount { get; set; }

    /// <summary>
    /// Number of files changed in the pull request
    /// </summary>
    public int ChangedFilesCount { get; set; }


    // Navigation properties

    /// <summary>
    /// The repository this pull request belongs to
    /// </summary>
    public Repository Repository { get; set; } = null!;

    /// <summary>
    /// The developer who created this pull request
    /// </summary>
    public Developer Author { get; set; } = null!;

}