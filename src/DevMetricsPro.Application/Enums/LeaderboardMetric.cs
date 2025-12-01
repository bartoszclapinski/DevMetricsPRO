namespace DevMetricsPro.Application.Enums;

/// <summary>
/// Metrics available for leaderboard ranking
/// </summary>
public enum LeaderboardMetric
{
    /// <summary>
    /// Total number of commits
    /// </summary>
    Commits,

    /// <summary>
    /// Total number of pull requests
    /// </summary>
    PullRequests,

    /// <summary>
    /// Total lines of code changed (additions + deletions)
    /// </summary>
    LinesChanged,

    /// <summary>
    /// Number of days with at least one contribution
    /// </summary>
    ActiveDays
}

