using DevMetricsPro.Application.DTOs.Metrics;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for calculating developer metrics from GitHub data
/// </summary>
public interface IMetricsCalculationService
{
    /// <summary>
    /// Calculate metrics for a specific developer within a date range
    /// </summary>
    /// <param name="developerId">The developer ID to calculate metrics for</param>
    /// <param name="startDate">Start date of the calculation period</param>
    /// <param name="endDate">End date of the calculation period</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CalculateMetricsForDeveloperAsync(
        Guid developerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate metrics for all developers using default date range (last 30 days)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CalculateMetricsForAllDevelopersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate PR review time metrics
    /// </summary>
    /// <param name="developerId">Optional developer ID (null = all developers)</param>
    /// <param name="startDate">Start date of analysis period</param>
    /// <param name="endDate">End date of analysis period</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<ReviewTimeMetricsDto> GetReviewTimeMetricsAsync(
        Guid? developerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate code velocity metrics (commits, lines, PRs per week)
    /// </summary>
    /// <param name="developerId">Optional developer ID (null = all developers)</param>
    /// <param name="numberOfWeeks">Number of weeks to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<CodeVelocityDto> GetCodeVelocityAsync(
        Guid? developerId,
        int numberOfWeeks,
        CancellationToken cancellationToken = default);
}
