using DevMetricsPro.Application.DTOs;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for sending real-time notifications via SignalR
/// </summary>
public interface IMetricsHubService
{
    /// <summary>
    /// Notify clients that metrics have been updated
    /// </summary>
    /// <param name="userId">The user ID whose metrics were updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task NotifyMetricsUpdatedAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify clients that a sync operation has completed
    /// </summary>
    /// <param name="userId">The user ID whose data was synced</param>
    /// <param name="result">The sync result details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task NotifySyncCompletedAsync(Guid userId, SyncResultDto result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify clients that a sync operation has started
    /// </summary>
    /// <param name="userId">The user ID whose data is being synced</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task NotifySyncStartedAsync(Guid userId, CancellationToken cancellationToken = default);
}


