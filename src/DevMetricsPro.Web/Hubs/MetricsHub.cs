using Microsoft.AspNetCore.SignalR;

namespace DevMetricsPro.Web.Hubs;

/// <summary>
/// SignalR hub for real-time metrics updates.
/// Clients connect to receive notifications when data syncs complete.
/// </summary>
public class MetricsHub : Hub
{
    private readonly ILogger<MetricsHub> _logger;

    public MetricsHub(ILogger<MetricsHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation(
            "Client disconnected: {ConnectionId}. Reason: {Reason}",
            Context.ConnectionId,
            exception?.Message ?? "Normal disconnect");
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Allows a client to join a user-specific group for receiving updates.
    /// </summary>
    /// <param name="userId">The user ID to subscribe to</param>
    public async Task JoinDashboard(string userId)
    {
        var groupName = $"user-{userId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation(
            "Client {ConnectionId} joined dashboard group {GroupName}",
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Allows a client to leave a user-specific group.
    /// </summary>
    /// <param name="userId">The user ID to unsubscribe from</param>
    public async Task LeaveDashboard(string userId)
    {
        var groupName = $"user-{userId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation(
            "Client {ConnectionId} left dashboard group {GroupName}",
            Context.ConnectionId, groupName);
    }
}


