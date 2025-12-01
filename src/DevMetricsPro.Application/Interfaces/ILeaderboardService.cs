using DevMetricsPro.Application.DTOs;
using DevMetricsPro.Application.Enums;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for retrieving leaderboard data
/// </summary>
public interface ILeaderboardService
{
    /// <summary>
    /// Get top contributors for a specific metric
    /// </summary>
    /// <param name="metric">The metric to rank by</param>
    /// <param name="topN">Number of top entries to return</param>
    /// <param name="startDate">Start of the time range</param>
    /// <param name="endDate">End of the time range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of leaderboard entries sorted by rank</returns>
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(
        LeaderboardMetric metric,
        int topN,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}

