namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Represents a commit in a source code repository
/// </summary>
public class Commit : BaseEntity
{
    /// <summary>
    /// Foreign key to the repository
    /// </summary>
    public Guid RepositoryId { get; set; }

    /// <summary>
    /// Foreign key to the developer who made the commit
    /// </summary>
    public Guid DeveloperId { get; set; }

    /// <summary>
    /// Git commit SHA (unique identifier)
    /// </summary>
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Commit message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Number of lines added in this commit
    /// </summary>
    public int LinesAdded { get; set; }

    /// <summary>
    /// Number of lines removed in this commit
    /// </summary>
    public int LinesRemoved { get; set; }

    /// <summary>
    /// Number of files changed in this commit
    /// </summary>
    public int FilesChanged { get; set; }

    /// <summary>
    /// When the commit was made (Git commit timestamp)
    /// </summary>
    public DateTime CommittedAt { get; set; }


    // Navigation properties

    /// <summary>
    /// The repository this commit belongs to
    /// </summary>
    public Repository Repository { get; set; } = null!;

    /// <summary>
    /// The developer who made this commit
    /// </summary>
    public Developer Developer { get; set; } = null!;
}