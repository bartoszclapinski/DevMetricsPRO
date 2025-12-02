namespace DevMetricsPro.Application.DTOs.Metrics;

/// <summary>
/// Metrics related to pull request review times
/// </summary>
public record ReviewTimeMetricsDto
{
    /// <summary>
    /// Average time from PR open to merge (in hours)
    /// </summary>
    public double AverageTimeToMergeHours { get; init; }

    /// <summary>
    /// Average time from PR open to merge (formatted string)
    /// </summary>
    public string AverageTimeToMergeFormatted => FormatDuration(AverageTimeToMergeHours);

    /// <summary>
    /// Median time to merge (in hours)
    /// </summary>
    public double MedianTimeToMergeHours { get; init; }

    /// <summary>
    /// Fastest PR merge time (in hours)
    /// </summary>
    public double FastestMergeHours { get; init; }

    /// <summary>
    /// Slowest PR merge time (in hours)
    /// </summary>
    public double SlowestMergeHours { get; init; }

    /// <summary>
    /// Total PRs analyzed
    /// </summary>
    public int TotalPRsAnalyzed { get; init; }

    /// <summary>
    /// Number of PRs merged
    /// </summary>
    public int MergedPRs { get; init; }

    /// <summary>
    /// Merge rate percentage (merged / total)
    /// </summary>
    public double MergeRatePercent { get; init; }

    /// <summary>
    /// Start date of the analysis period
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// End date of the analysis period
    /// </summary>
    public DateTime EndDate { get; init; }

    private static string FormatDuration(double hours)
    {
        if (hours < 1)
            return $"{(int)(hours * 60)}m";
        if (hours < 24)
            return $"{hours:F1}h";
        return $"{(hours / 24):F1}d";
    }

    public static ReviewTimeMetricsDto Empty(DateTime startDate, DateTime endDate) => new()
    {
        AverageTimeToMergeHours = 0,
        MedianTimeToMergeHours = 0,
        FastestMergeHours = 0,
        SlowestMergeHours = 0,
        TotalPRsAnalyzed = 0,
        MergedPRs = 0,
        MergeRatePercent = 0,
        StartDate = startDate,
        EndDate = endDate
    };
}

