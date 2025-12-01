using DevMetricsPro.Application.DTOs;
using DevMetricsPro.Application.Enums;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevMetricsPro.Application.Services;

/// <summary>
/// Service for retrieving leaderboard data
/// </summary>
public class LeaderboardService : ILeaderboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LeaderboardService> _logger;

    public LeaderboardService(
        IUnitOfWork unitOfWork,
        ILogger<LeaderboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(
        LeaderboardMetric metric,
        int topN,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching leaderboard for {Metric}, top {TopN}, from {StartDate} to {EndDate}",
            metric, topN, startDate, endDate);

        return metric switch
        {
            LeaderboardMetric.Commits => await GetCommitsLeaderboardAsync(topN, startDate, endDate, cancellationToken),
            LeaderboardMetric.PullRequests => await GetPullRequestsLeaderboardAsync(topN, startDate, endDate, cancellationToken),
            LeaderboardMetric.LinesChanged => await GetLinesChangedLeaderboardAsync(topN, startDate, endDate, cancellationToken),
            LeaderboardMetric.ActiveDays => await GetActiveDaysLeaderboardAsync(topN, startDate, endDate, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, "Unknown leaderboard metric")
        };
    }

    private async Task<List<LeaderboardEntryDto>> GetCommitsLeaderboardAsync(
        int topN,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var leaderboard = await _unitOfWork.Repository<Commit>()
            .Query()
            .AsNoTracking()
            .Where(c => c.CommittedAt >= startDate && c.CommittedAt <= endDate)
            .GroupBy(c => c.Developer)
            .Select(g => new
            {
                Developer = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToListAsync(cancellationToken);

        return leaderboard
            .Select((entry, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                DeveloperId = entry.Developer.Id,
                DeveloperName = entry.Developer.DisplayName ?? entry.Developer.GitHubUsername ?? entry.Developer.Email,
                AvatarUrl = entry.Developer.AvatarUrl,
                Value = entry.Count,
                Change = "0", // TODO: Calculate trend from previous period
                Trend = TrendDirection.Neutral
            })
            .ToList();
    }

    private async Task<List<LeaderboardEntryDto>> GetPullRequestsLeaderboardAsync(
        int topN,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var leaderboard = await _unitOfWork.Repository<PullRequest>()
            .Query()
            .AsNoTracking()
            .Where(pr => pr.CreatedAt >= startDate && pr.CreatedAt <= endDate)
            .GroupBy(pr => pr.Author)
            .Select(g => new
            {
                Developer = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToListAsync(cancellationToken);

        return leaderboard
            .Select((entry, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                DeveloperId = entry.Developer.Id,
                DeveloperName = entry.Developer.DisplayName ?? entry.Developer.GitHubUsername ?? entry.Developer.Email,
                AvatarUrl = entry.Developer.AvatarUrl,
                Value = entry.Count,
                Change = "0",
                Trend = TrendDirection.Neutral
            })
            .ToList();
    }

    private async Task<List<LeaderboardEntryDto>> GetLinesChangedLeaderboardAsync(
        int topN,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var leaderboard = await _unitOfWork.Repository<Commit>()
            .Query()
            .AsNoTracking()
            .Where(c => c.CommittedAt >= startDate && c.CommittedAt <= endDate)
            .GroupBy(c => c.Developer)
            .Select(g => new
            {
                Developer = g.Key,
                LinesChanged = g.Sum(c => c.LinesAdded + c.LinesRemoved)
            })
            .OrderByDescending(x => x.LinesChanged)
            .Take(topN)
            .ToListAsync(cancellationToken);

        return leaderboard
            .Select((entry, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                DeveloperId = entry.Developer.Id,
                DeveloperName = entry.Developer.DisplayName ?? entry.Developer.GitHubUsername ?? entry.Developer.Email,
                AvatarUrl = entry.Developer.AvatarUrl,
                Value = entry.LinesChanged,
                Change = "0",
                Trend = TrendDirection.Neutral
            })
            .ToList();
    }

    private async Task<List<LeaderboardEntryDto>> GetActiveDaysLeaderboardAsync(
        int topN,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var leaderboard = await _unitOfWork.Repository<Commit>()
            .Query()
            .AsNoTracking()
            .Where(c => c.CommittedAt >= startDate && c.CommittedAt <= endDate)
            .GroupBy(c => c.Developer)
            .Select(g => new
            {
                Developer = g.Key,
                ActiveDays = g.Select(c => c.CommittedAt.Date).Distinct().Count()
            })
            .OrderByDescending(x => x.ActiveDays)
            .Take(topN)
            .ToListAsync(cancellationToken);

        return leaderboard
            .Select((entry, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                DeveloperId = entry.Developer.Id,
                DeveloperName = entry.Developer.DisplayName ?? entry.Developer.GitHubUsername ?? entry.Developer.Email,
                AvatarUrl = entry.Developer.AvatarUrl,
                Value = entry.ActiveDays,
                Change = "0",
                Trend = TrendDirection.Neutral
            })
            .ToList();
    }
}

