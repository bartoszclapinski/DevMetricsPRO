using DevMetricsPro.Application.DTOs.Metrics;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Enums;
using DevMetricsPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// Calculate PR review time metrics
    /// </summary>
    public async Task<ReviewTimeMetricsDto> GetReviewTimeMetricsAsync(
        Guid? developerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calculating review time metrics for developer {DeveloperId} from {StartDate} to {EndDate}",
            developerId?.ToString() ?? "all", startDate, endDate);

        try
        {
            var prRepo = _unitOfWork.Repository<PullRequest>();

            // Build query
            var query = prRepo.Query().AsNoTracking();

            if (developerId.HasValue)
            {
                query = query.Where(pr => pr.AuthorId == developerId.Value);
            }

            query = query.Where(pr => pr.CreatedAt >= startDate && pr.CreatedAt <= endDate);

            var pullRequests = await query.ToListAsync(cancellationToken);

            if (!pullRequests.Any())
            {
                return ReviewTimeMetricsDto.Empty(startDate, endDate);
            }

            // Get merged PRs with time to merge
            var mergedPRs = pullRequests
                .Where(pr => pr.Status == PullRequestStatus.Merged && pr.MergedAt.HasValue)
                .Select(pr => new
                {
                    PR = pr,
                    TimeToMergeHours = (pr.MergedAt!.Value - pr.CreatedAt).TotalHours
                })
                .ToList();

            if (!mergedPRs.Any())
            {
                return new ReviewTimeMetricsDto
                {
                    TotalPRsAnalyzed = pullRequests.Count,
                    MergedPRs = 0,
                    MergeRatePercent = 0,
                    StartDate = startDate,
                    EndDate = endDate
                };
            }

            var mergeTimes = mergedPRs.Select(x => x.TimeToMergeHours).OrderBy(x => x).ToList();

            return new ReviewTimeMetricsDto
            {
                AverageTimeToMergeHours = mergeTimes.Average(),
                MedianTimeToMergeHours = GetMedian(mergeTimes),
                FastestMergeHours = mergeTimes.First(),
                SlowestMergeHours = mergeTimes.Last(),
                TotalPRsAnalyzed = pullRequests.Count,
                MergedPRs = mergedPRs.Count,
                MergeRatePercent = (double)mergedPRs.Count / pullRequests.Count * 100,
                StartDate = startDate,
                EndDate = endDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating review time metrics");
            throw;
        }
    }

    /// <summary>
    /// Calculate code velocity metrics (commits, lines, PRs per week)
    /// </summary>
    public async Task<CodeVelocityDto> GetCodeVelocityAsync(
        Guid? developerId,
        int numberOfWeeks,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calculating code velocity for developer {DeveloperId} over {Weeks} weeks",
            developerId?.ToString() ?? "all", numberOfWeeks);

        try
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-numberOfWeeks * 7);

            var commitRepo = _unitOfWork.Repository<Commit>();
            var prRepo = _unitOfWork.Repository<PullRequest>();

            // Get commits
            var commitQuery = commitRepo.Query().AsNoTracking()
                .Where(c => c.CommittedAt >= startDate && c.CommittedAt <= endDate);

            if (developerId.HasValue)
            {
                commitQuery = commitQuery.Where(c => c.DeveloperId == developerId.Value);
            }

            var commits = await commitQuery.ToListAsync(cancellationToken);

            // Get merged PRs
            var prQuery = prRepo.Query().AsNoTracking()
                .Where(pr => pr.MergedAt >= startDate && pr.MergedAt <= endDate && pr.Status == PullRequestStatus.Merged);

            if (developerId.HasValue)
            {
                prQuery = prQuery.Where(pr => pr.AuthorId == developerId.Value);
            }

            var mergedPRs = await prQuery.ToListAsync(cancellationToken);

            if (!commits.Any() && !mergedPRs.Any())
            {
                return CodeVelocityDto.Empty(startDate, endDate);
            }

            // Group by week
            var weeklyData = new List<WeeklyVelocity>();
            var currentWeekStart = GetWeekStart(startDate);

            while (currentWeekStart < endDate)
            {
                var weekEnd = currentWeekStart.AddDays(7);

                var weekCommits = commits
                    .Where(c => c.CommittedAt >= currentWeekStart && c.CommittedAt < weekEnd)
                    .ToList();

                var weekPRs = mergedPRs
                    .Where(pr => pr.MergedAt >= currentWeekStart && pr.MergedAt < weekEnd)
                    .ToList();

                weeklyData.Add(new WeeklyVelocity
                {
                    WeekStart = currentWeekStart,
                    Label = currentWeekStart.ToString("MMM dd"),
                    Commits = weekCommits.Count,
                    LinesAdded = weekCommits.Sum(c => c.LinesAdded),
                    LinesRemoved = weekCommits.Sum(c => c.LinesRemoved),
                    PRsMerged = weekPRs.Count,
                    ActiveDays = weekCommits.Select(c => c.CommittedAt.Date).Distinct().Count()
                });

                currentWeekStart = weekEnd;
            }

            // Calculate trend (compare last half to first half)
            var (trend, trendPercent) = CalculateTrend(weeklyData);

            return new CodeVelocityDto
            {
                WeeklyData = weeklyData,
                AverageCommitsPerWeek = weeklyData.Any() ? weeklyData.Average(w => w.Commits) : 0,
                AverageLinesPerWeek = weeklyData.Any() ? weeklyData.Average(w => w.TotalLinesChanged) : 0,
                AveragePRsPerWeek = weeklyData.Any() ? weeklyData.Average(w => w.PRsMerged) : 0,
                CommitTrend = trend,
                CommitTrendPercent = trendPercent,
                TotalCommits = commits.Count,
                TotalLinesChanged = commits.Sum(c => c.LinesAdded + c.LinesRemoved),
                TotalPRsMerged = mergedPRs.Count,
                WeeksAnalyzed = weeklyData.Count,
                StartDate = startDate,
                EndDate = endDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating code velocity");
            throw;
        }
    }

    private static double GetMedian(List<double> values)
    {
        var count = values.Count;
        if (count == 0) return 0;
        if (count % 2 == 0)
        {
            return (values[count / 2 - 1] + values[count / 2]) / 2;
        }
        return values[count / 2];
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    private static (int trend, double trendPercent) CalculateTrend(List<WeeklyVelocity> weeklyData)
    {
        if (weeklyData.Count < 4) return (0, 0);

        var halfIndex = weeklyData.Count / 2;
        var firstHalf = weeklyData.Take(halfIndex).Average(w => w.Commits);
        var secondHalf = weeklyData.Skip(halfIndex).Average(w => w.Commits);

        if (firstHalf == 0) return (secondHalf > 0 ? 1 : 0, 0);

        var changePercent = ((secondHalf - firstHalf) / firstHalf) * 100;
        var trend = changePercent > 5 ? 1 : (changePercent < -5 ? -1 : 0);

        return (trend, changePercent);
    }
}
