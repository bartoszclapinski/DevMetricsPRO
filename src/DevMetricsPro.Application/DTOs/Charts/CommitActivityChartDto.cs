namespace DevMetricsPro.Application.DTOs.Charts;

/// <summary>
/// Data transfer object for commit activity chart
/// </summary>
public record CommitActivityChartDto
{
    /// <summary>
    /// Date labels for X-axis (e.g., "Nov 20", "Nov 21")
    /// </summary>
    public List<string> Labels { get; init; } = new();

    /// <summary>
    /// Commit counts for Y-axis (e.g., 12, 19, 8)
    /// </summary>
    public List<int> Values { get; init; } = new();

    /// <summary>
    /// Total number of commits in the time range
    /// </summary>
    public int TotalCommits { get; init; }

    /// <summary>
    /// Average commits per day in the time range
    /// </summary>
    public double AveragePerDay { get; init; }

    /// <summary>
    /// Date range start
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Date range end
    /// </summary>
    public DateTime EndDate { get; init; }
}