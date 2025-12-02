namespace DevMetricsPro.Application.DTOs.Metrics;

/// <summary>
/// Code velocity metrics showing productivity trends over time
/// </summary>
public record CodeVelocityDto
{
    /// <summary>
    /// Weekly breakdown of velocity metrics
    /// </summary>
    public List<WeeklyVelocity> WeeklyData { get; init; } = new();

    /// <summary>
    /// Average commits per week over the period
    /// </summary>
    public double AverageCommitsPerWeek { get; init; }

    /// <summary>
    /// Average lines changed per week
    /// </summary>
    public double AverageLinesPerWeek { get; init; }

    /// <summary>
    /// Average PRs merged per week
    /// </summary>
    public double AveragePRsPerWeek { get; init; }

    /// <summary>
    /// Trend direction for commits (-1 = declining, 0 = stable, 1 = increasing)
    /// </summary>
    public int CommitTrend { get; init; }

    /// <summary>
    /// Percentage change in commits (comparing recent weeks to earlier weeks)
    /// </summary>
    public double CommitTrendPercent { get; init; }

    /// <summary>
    /// Total commits in the period
    /// </summary>
    public int TotalCommits { get; init; }

    /// <summary>
    /// Total lines changed in the period
    /// </summary>
    public int TotalLinesChanged { get; init; }

    /// <summary>
    /// Total PRs merged in the period
    /// </summary>
    public int TotalPRsMerged { get; init; }

    /// <summary>
    /// Number of weeks analyzed
    /// </summary>
    public int WeeksAnalyzed { get; init; }

    /// <summary>
    /// Start date of the analysis period
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// End date of the analysis period
    /// </summary>
    public DateTime EndDate { get; init; }

    public static CodeVelocityDto Empty(DateTime startDate, DateTime endDate) => new()
    {
        WeeklyData = new List<WeeklyVelocity>(),
        AverageCommitsPerWeek = 0,
        AverageLinesPerWeek = 0,
        AveragePRsPerWeek = 0,
        CommitTrend = 0,
        CommitTrendPercent = 0,
        TotalCommits = 0,
        TotalLinesChanged = 0,
        TotalPRsMerged = 0,
        WeeksAnalyzed = 0,
        StartDate = startDate,
        EndDate = endDate
    };
}

/// <summary>
/// Velocity metrics for a single week
/// </summary>
public record WeeklyVelocity
{
    /// <summary>
    /// Start date of the week (Monday)
    /// </summary>
    public DateTime WeekStart { get; init; }

    /// <summary>
    /// Week label (e.g., "Nov 25")
    /// </summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>
    /// Number of commits this week
    /// </summary>
    public int Commits { get; init; }

    /// <summary>
    /// Lines added this week
    /// </summary>
    public int LinesAdded { get; init; }

    /// <summary>
    /// Lines removed this week
    /// </summary>
    public int LinesRemoved { get; init; }

    /// <summary>
    /// Total lines changed (added + removed)
    /// </summary>
    public int TotalLinesChanged => LinesAdded + LinesRemoved;

    /// <summary>
    /// PRs merged this week
    /// </summary>
    public int PRsMerged { get; init; }

    /// <summary>
    /// Number of active days (days with commits)
    /// </summary>
    public int ActiveDays { get; init; }
}

