namespace DevMetricsPro.Application.DTOs;

/// <summary>
/// Result of a GitHub data sync operation
/// </summary>
public record SyncResultDto
{
    /// <summary>
    /// Number of repositories synced
    /// </summary>
    public int RepositoriesSynced { get; init; }

    /// <summary>
    /// Number of commits synced
    /// </summary>
    public int CommitsSynced { get; init; }

    /// <summary>
    /// Number of pull requests synced
    /// </summary>
    public int PullRequestsSynced { get; init; }

    /// <summary>
    /// Whether metrics were calculated successfully
    /// </summary>
    public bool MetricsCalculated { get; init; }

    /// <summary>
    /// Timestamp when sync completed
    /// </summary>
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the sync was successful
    /// </summary>
    public bool Success { get; init; } = true;

    /// <summary>
    /// Error message if sync failed
    /// </summary>
    public string? ErrorMessage { get; init; }
}


