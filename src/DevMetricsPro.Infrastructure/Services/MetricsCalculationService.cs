using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Enums;
using DevMetricsPro.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for calculating developer metrics from GitHub data (commits and PRs)
/// </summary>
public class MetricsCalculationService : IMetricsCalculationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MetricsCalculationService> _logger;

    public MetricsCalculationService(
        IUnitOfWork unitOfWork,
        ILogger<MetricsCalculationService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculate metrics for a specific developer within a date range
    /// </summary>
    public async Task CalculateMetricsForDeveloperAsync(
        Guid developerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calculating metrics for developer {DeveloperId} from {StartDate} to {EndDate}",
            developerId, startDate, endDate);

        try
        {
            // Get repositories
            var commitRepo = _unitOfWork.Repository<Commit>();
            var prRepo = _unitOfWork.Repository<PullRequest>();
            var metricRepo = _unitOfWork.Repository<Metric>();

            // Get all commits for developer in date range
            var commits = (await commitRepo.FindAsync(
                    c => c.DeveloperId == developerId &&
                         c.CommittedAt >= startDate &&
                         c.CommittedAt <= endDate,
                    cancellationToken))
                .ToList();

            // Get all PRs for developer in date range
            var pullRequests = (await prRepo.FindAsync(
                    pr => pr.AuthorId == developerId &&
                          pr.CreatedAt >= startDate &&
                          pr.CreatedAt <= endDate,
                    cancellationToken))
                .ToList();

            _logger.LogDebug(
                "Found {CommitCount} commits and {PRCount} PRs for developer {DeveloperId}",
                commits.Count, pullRequests.Count, developerId);

            // Calculate 5 metrics
            var totalCommits = commits.Count;
            var linesAdded = commits.Sum(c => c.LinesAdded);
            var linesRemoved = commits.Sum(c => c.LinesRemoved);
            var prCount = pullRequests.Count;
            var activeDays = commits
                .Select(c => c.CommittedAt.Date)
                .Distinct()
                .Count();

            // Prepare metadata JSON
            var metadata = $"{{\"startDate\":\"{startDate:yyyy-MM-dd}\",\"endDate\":\"{endDate:yyyy-MM-dd}\"}}";

            // Store each metric (upsert logic)
            await StoreMetricAsync(developerId, MetricType.Commits, totalCommits, metadata, cancellationToken);
            await StoreMetricAsync(developerId, MetricType.LinesAdded, linesAdded, metadata, cancellationToken);
            await StoreMetricAsync(developerId, MetricType.LinesRemoved, linesRemoved, metadata, cancellationToken);
            await StoreMetricAsync(developerId, MetricType.PullRequests, prCount, metadata, cancellationToken);
            await StoreMetricAsync(developerId, MetricType.ActiveDays, activeDays, metadata, cancellationToken);

            // Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully calculated metrics for developer {DeveloperId}: " +
                "Commits={Commits}, LinesAdded={LinesAdded}, LinesRemoved={LinesRemoved}, " +
                "PRs={PRs}, ActiveDays={ActiveDays}",
                developerId, totalCommits, linesAdded, linesRemoved, prCount, activeDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating metrics for developer {DeveloperId}", developerId);
            throw;
        }
    }

    /// <summary>
    /// Calculate metrics for all developers using default date range (last 30 days)
    /// </summary>
    public async Task CalculateMetricsForAllDevelopersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting metrics calculation for all developers");

        try
        {
            // Define date range: last 30 days
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            _logger.LogInformation("Using date range: {StartDate} to {EndDate}", startDate, endDate);

            // Get all developers
            var developerRepo = _unitOfWork.Repository<Developer>();
            var developerList = (await developerRepo.GetAllAsync(cancellationToken)).ToList();

            _logger.LogInformation("Found {Count} developers to process", developerList.Count);

            var successCount = 0;
            var errorCount = 0;

            // Calculate metrics for each developer
            foreach (var developer in developerList)
            {
                try
                {
                    await CalculateMetricsForDeveloperAsync(
                        developer.Id,
                        startDate,
                        endDate,
                        cancellationToken);

                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to calculate metrics for developer {DeveloperId} ({Email})",
                        developer.Id, developer.Email);
                    errorCount++;
                    // Continue processing other developers
                }
            }

            _logger.LogInformation(
                "Metrics calculation completed: {Success} succeeded, {Errors} failed",
                successCount, errorCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in metrics calculation for all developers");
            throw;
        }
    }

    /// <summary>
    /// Store or update a metric (upsert logic)
    /// </summary>
    private async Task StoreMetricAsync(
        Guid developerId,
        MetricType metricType,
        decimal value,
        string metadata,
        CancellationToken cancellationToken)
    {
        var metricRepo = _unitOfWork.Repository<Metric>();

        // Check if metric exists for this developer and type
        var existingMetric = (await metricRepo.FindAsync(
                m => m.DeveloperId == developerId &&
                     m.Type == metricType,
                cancellationToken))
            .FirstOrDefault();

        if (existingMetric != null)
        {
            // Update existing metric
            existingMetric.Value = value;
            existingMetric.Timestamp = DateTime.UtcNow;
            existingMetric.Metadata = metadata;
            existingMetric.UpdatedAt = DateTime.UtcNow;

            await metricRepo.UpdateAsync(existingMetric, cancellationToken);

            _logger.LogDebug(
                "Updated metric {MetricType} for developer {DeveloperId}: {Value}",
                metricType, developerId, value);
        }
        else
        {
            // Create new metric
            var newMetric = new Metric
            {
                Id = Guid.NewGuid(),
                DeveloperId = developerId,
                Type = metricType,
                Value = value,
                Timestamp = DateTime.UtcNow,
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow
            };

            await metricRepo.AddAsync(newMetric, cancellationToken);

            _logger.LogDebug(
                "Created metric {MetricType} for developer {DeveloperId}: {Value}",
                metricType, developerId, value);
        }
    }
}
