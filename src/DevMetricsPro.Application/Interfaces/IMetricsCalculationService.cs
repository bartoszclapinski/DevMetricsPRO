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
}
