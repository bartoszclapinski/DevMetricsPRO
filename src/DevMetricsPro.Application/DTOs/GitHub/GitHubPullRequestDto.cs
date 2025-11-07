namespace DevMetricsPro.Application.DTOs.GitHub;

/// <summary>
/// DTO for GitHub pull request information
/// </summary>
public class GitHubPullRequestDto
{
    /// <summary>
    /// Pull request number (unique per repository)
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Pull request title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Pull request state (Open, Closed)
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Pull request body/description
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Link to pull request on GitHub
    /// </summary>
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>
    /// Author username
    /// </summary>
    public string AuthorLogin { get; set; } = string.Empty;

    /// <summary>
    /// Author display name
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// When the PR was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the PR was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// When the PR was closed (if applicable)
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// When the PR was merged (if applicable)
    /// </summary>
    public DateTime? MergedAt { get; set; }

    /// <summary>
    /// Whether the PR was merged
    /// </summary>
    public bool IsMerged { get; set; }

    /// <summary>
    /// Whether the PR is a draft
    /// </summary>
    public bool IsDraft { get; set; }

    /// <summary>
    /// Number of lines added in this PR
    /// </summary>
    public int Additions { get; set; }

    /// <summary>
    /// Number of lines deleted in this PR
    /// </summary>
    public int Deletions { get; set; }

    /// <summary>
    /// Number of files changed in this PR
    /// </summary>
    public int ChangedFiles { get; set; }

    /// <summary>
    /// Repository name for reference
    /// </summary>
    public string RepositoryName { get; set; } = string.Empty;
}

