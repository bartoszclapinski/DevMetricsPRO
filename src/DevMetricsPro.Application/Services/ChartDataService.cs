using DevMetricsPro.Application.DTOs.Charts;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
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
}