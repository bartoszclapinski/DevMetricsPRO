using DevMetricsPro.Application.DTOs.Charts;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Enums;
using DevMetricsPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevMetricsPro.Application.Services;

public class ChartDataService : IChartDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChartDataService> _logger;

    public ChartDataService(
        IUnitOfWork unitOfWork,
        ILogger<ChartDataService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CommitActivityChartDto> GetCommitActivityAsync(
        Guid? developerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Fetching commit activity from {StartDate} to {EndDate} for developer {DeveloperId}",
                startDate, endDate, developerId);

            // Get all commits in the date range
            var query = _unitOfWork.Repository<Commit>()
                .Query()
                .AsNoTracking()
                .Where(c => c.CommittedAt >= startDate && c.CommittedAt <= endDate);

            // Filter by developer if specified
            if (developerId.HasValue)
            {
                query = query.Where(c => c.DeveloperId == developerId.Value);
            }

            // Group by date and count
            var commitsByDate = await query
                .GroupBy(c => c.CommittedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            // Calculate total days in range
            var totalDays = (endDate.Date - startDate.Date).Days + 1;

            // Fill in missing dates with zero commits
            var allDates = Enumerable.Range(0, totalDays)
                .Select(offset => startDate.Date.AddDays(offset))
                .ToList();

            var labels = new List<string>();
            var values = new List<int>();

            foreach (var date in allDates)
            {
                var commitData = commitsByDate.FirstOrDefault(x => x.Date == date);
                
                labels.Add(date.ToString("MMM dd")); // e.g., "Nov 23"
                values.Add(commitData?.Count ?? 0);
            }

            var totalCommits = values.Sum();
            var averagePerDay = totalDays > 0 ? (double)totalCommits / totalDays : 0;

            _logger.LogInformation(
                "Found {TotalCommits} commits over {TotalDays} days (avg: {Average:F2}/day)",
                totalCommits, totalDays, averagePerDay);

            return new CommitActivityChartDto
            {
                Labels = labels,
                Values = values,
                TotalCommits = totalCommits,
                AveragePerDay = Math.Round(averagePerDay, 2),
                StartDate = startDate,
                EndDate = endDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching commit activity data");
            throw;
        }
    }

    public async Task<PullRequestChartDto> GetPullRequestStatsAsync(
        int days = 30,
        Guid? developerId = null,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);

        try
        {
            _logger.LogInformation(
                "Fetching PR statistics from {StartDate} to {EndDate} for developer {DeveloperId}",
                startDate, endDate, developerId);

            // Get PRs in the date range
            var query = _unitOfWork.Repository<PullRequest>()
                .Query()
                .AsNoTracking()
                .Where(pr => pr.CreatedAt >= startDate && pr.CreatedAt <= endDate);

            // Filter by developer if specified
            if (developerId.HasValue)
            {
                query = query.Where(pr => pr.AuthorId == developerId.Value);
            }

            var pullRequests = await query.ToListAsync(cancellationToken);

            if (!pullRequests.Any())
            {
                _logger.LogInformation("No PRs found in the date range");
                return PullRequestChartDto.CreateEmpty(startDate, endDate);
            }

            // Group by status
            var statusGroups = pullRequests
                .GroupBy(pr => pr.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToList();

            // Calculate average review time for merged PRs
            double? avgReviewTime = null;
            var mergedPRs = pullRequests
                .Where(pr => pr.MergedAt.HasValue && pr.CreatedAt != default)
                .ToList();

            if (mergedPRs.Any())
            {
                var reviewTimes = mergedPRs
                    .Select(pr => (pr.MergedAt!.Value - pr.CreatedAt).TotalHours)
                    .Where(hours => hours > 0);

                if (reviewTimes.Any())
                {
                    avgReviewTime = reviewTimes.Average();
                }
            }

            _logger.LogInformation(
                "Found {TotalPRs} PRs with {MergedCount} merged (avg review time: {AvgReviewTime:F1}h)",
                pullRequests.Count, mergedPRs.Count, avgReviewTime ?? 0);

            return new PullRequestChartDto
            {
                Labels = statusGroups.Select(g => g.Status.ToString()).ToList(),
                Values = statusGroups.Select(g => g.Count).ToList(),
                TotalPRs = pullRequests.Count,
                AverageReviewTimeHours = avgReviewTime,
                StartDate = startDate,
                EndDate = endDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching PR statistics");
            throw;
        }
    }
}