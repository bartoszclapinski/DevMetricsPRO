using DevMetricsPro.Application.DTOs.Charts;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for generating chart data from application data
/// </summary>
public interface IChartDataService
{
    /// <summary>
    /// Get commit activity data for chart visualization
    /// </summary>
    /// <param name="developerId">Optional developer ID to filter by specific developer</param>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Chart data with labels and values</returns>
    Task<CommitActivityChartDto> GetCommitActivityAsync(
        Guid? developerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pull request statistics for chart display
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    /// <param name="developerId">Optional developer ID to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PR statistics with status breakdown and average review time</returns>
    Task<PullRequestChartDto> GetPullRequestStatsAsync(
        int days = 30,
        Guid? developerId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get contribution heatmap data for GitHub-style visualization
    /// </summary>
    /// <param name="numberOfWeeks">Number of weeks to display (default: 52)</param>
    /// <param name="developerId">Optional developer ID to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Heatmap data with daily contribution counts and levels</returns>
    Task<ContributionHeatmapDto> GetContributionHeatmapAsync(
        int numberOfWeeks = 52,
        Guid? developerId = null,
        CancellationToken cancellationToken = default);
}