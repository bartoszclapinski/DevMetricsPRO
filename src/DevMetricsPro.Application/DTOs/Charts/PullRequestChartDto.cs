namespace DevMetricsPro.Application.DTOs.Charts;

/// <summary>
/// Data transfer object for pull request statistics chart
/// </summary>
public record PullRequestChartDto
{
    /// <summary>
    /// Chart labels (e.g., "Open", "Closed", "Merged")
    /// </summary>
    public required List<string> Labels { get; init; }

    /// <summary>
    /// Count of PRs for each status
    /// </summary>
    public required List<int> Values { get; init; }

    /// <summary>
    /// Total number of PRs
    /// </summary>
    public int TotalPRs { get; init; }

    /// <summary>
    /// Average PR review time in hours (null if no merged PRs)
    /// </summary>
    public double? AverageReviewTimeHours { get; init; }

    /// <summary>
    /// Date range start
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Date range end
    /// </summary>
    public DateTime EndDate { get; init; }

    /// <summary>
    /// Factory method to create an empty chart
    /// </summary>
    public static PullRequestChartDto CreateEmpty(DateTime startDate, DateTime endDate)
    {
        return new PullRequestChartDto
        {
            Labels = [],
            Values = [],
            TotalPRs = 0,
            AverageReviewTimeHours = null,
            StartDate = startDate,
            EndDate = endDate
        };
    }
}

