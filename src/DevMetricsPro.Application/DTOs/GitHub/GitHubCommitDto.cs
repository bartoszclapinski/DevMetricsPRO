namespace DevMetricsPro.Application.DTOs.GitHub;

/// <summary>
/// DTO for GitHub commit information
/// </summary>
public class GitHubCommitDto
{
    /// <summary>
    /// Commit SHA (unique identifier)
    /// </summary>
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Commit message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Commit author name
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Commit author email
    /// </summary>
    public string AuthorEmail { get; set; } = string.Empty;

    /// <summary>
    /// When the commit was authored
    /// </summary>
    public DateTime AuthorDate { get; set; }

    /// <summary>
    /// Committer name (can differ from author)
    /// </summary>
    public string CommitterName { get; set; } = string.Empty;

    /// <summary>
    /// Committer email (can differ from author)
    /// </summary>
    public string CommitterEmail { get; set; } = string.Empty;

    /// <summary>
    /// When the commit was committed
    /// </summary>
    public DateTime CommitterDate { get; set; }

    /// <summary>
    /// Link to commit on GitHub
    /// </summary>
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>
    /// Numbers of lines added in this commit
    /// </summary>
    public int Additions { get; set; }

    /// <summary>
    /// Numbers of lines removed in this commit
    /// </summary>
    public int Deletions { get; set; }

    /// <summary>
    /// Numbers of lines changed in this commit
    /// </summary>
    public int TotalChanges { get; set; }

    /// <summary>
    /// Repository name for reference
    /// </summary>
    public string RepositoryName { get; set; } = string.Empty;
    
}