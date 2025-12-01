using DevMetricsPro.Application.DTOs;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DevMetricsPro.Web.Services;

/// <summary>
/// Service for sending real-time notifications via SignalR.
/// Uses IHubContext to send messages from outside the hub.
/// </summary>
public class MetricsHubService : IMetricsHubService
{
    private readonly IHubContext<MetricsHub> _hubContext;
    private readonly ILogger<MetricsHubService> _logger;

    public MetricsHubService(
        IHubContext<MetricsHub> hubContext,
        ILogger<MetricsHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyMetricsUpdatedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groupName = $"user-{userId}";

        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync(
                "MetricsUpdated",
                new { Timestamp = DateTime.UtcNow },
                cancellationToken);

            _logger.LogInformation("Sent MetricsUpdated notification to group {GroupName}", groupName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send MetricsUpdated notification to group {GroupName}", groupName);
            // Don't throw - notifications are best-effort
        }
    }

    public async Task NotifySyncCompletedAsync(Guid userId, SyncResultDto result, CancellationToken cancellationToken = default)
    {
        var groupName = $"user-{userId}";

        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync(
                "SyncCompleted",
                result,
                cancellationToken);

            _logger.LogInformation(
                "Sent SyncCompleted notification to group {GroupName}. " +
                "Repos: {Repos}, Commits: {Commits}, PRs: {PRs}",
                groupName, result.RepositoriesSynced, result.CommitsSynced, result.PullRequestsSynced);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send SyncCompleted notification to group {GroupName}", groupName);
        }
    }

    public async Task NotifySyncStartedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groupName = $"user-{userId}";

        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync(
                "SyncStarted",
                new { Timestamp = DateTime.UtcNow },
                cancellationToken);

            _logger.LogInformation("Sent SyncStarted notification to group {GroupName}", groupName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send SyncStarted notification to group {GroupName}", groupName);
        }
    }
}


