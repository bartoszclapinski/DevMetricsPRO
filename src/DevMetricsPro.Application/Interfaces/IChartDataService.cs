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
}