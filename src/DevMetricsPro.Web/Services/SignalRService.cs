using DevMetricsPro.Application.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DevMetricsPro.Web.Services;

/// <summary>
/// Client-side SignalR service for real-time dashboard updates.
/// Manages connection lifecycle and event handling.
/// </summary>
public class SignalRService : IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<SignalRService> _logger;
    private HubConnection? _hubConnection;
    private string? _currentUserId;

    /// <summary>
    /// Fired when sync operation starts
    /// </summary>
    public event Func<Task>? OnSyncStarted;

    /// <summary>
    /// Fired when sync operation completes
    /// </summary>
    public event Func<SyncResultDto, Task>? OnSyncCompleted;

    /// <summary>
    /// Fired when metrics are updated
    /// </summary>
    public event Func<Task>? OnMetricsUpdated;

    /// <summary>
    /// Fired when connection state changes
    /// </summary>
    public event Action<HubConnectionState>? OnConnectionStateChanged;

    /// <summary>
    /// Current connection state
    /// </summary>
    public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;

    /// <summary>
    /// Whether the connection is established
    /// </summary>
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public SignalRService(
        NavigationManager navigationManager,
        ILogger<SignalRService> logger)
    {
        _navigationManager = navigationManager;
        _logger = logger;
    }

    /// <summary>
    /// Start the SignalR connection and join the user's dashboard group
    /// </summary>
    /// <param name="userId">The user ID to subscribe to updates for</param>
    public async Task StartAsync(string userId)
    {
        if (_hubConnection != null)
        {
            _logger.LogWarning("SignalR connection already exists, disposing old connection");
            await DisposeAsync();
        }

        _currentUserId = userId;

        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubs/metrics"))
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            // Register event handlers
            _hubConnection.On("SyncStarted", async (object data) =>
            {
                _logger.LogInformation("Received SyncStarted event");
                if (OnSyncStarted != null)
                {
                    await OnSyncStarted.Invoke();
                }
            });

            _hubConnection.On<SyncResultDto>("SyncCompleted", async (result) =>
            {
                _logger.LogInformation(
                    "Received SyncCompleted event. Repos: {Repos}, Commits: {Commits}, PRs: {PRs}",
                    result.RepositoriesSynced, result.CommitsSynced, result.PullRequestsSynced);
                
                if (OnSyncCompleted != null)
                {
                    await OnSyncCompleted.Invoke(result);
                }
            });

            _hubConnection.On("MetricsUpdated", async (object data) =>
            {
                _logger.LogInformation("Received MetricsUpdated event");
                if (OnMetricsUpdated != null)
                {
                    await OnMetricsUpdated.Invoke();
                }
            });

            // Connection state handlers
            _hubConnection.Reconnecting += error =>
            {
                _logger.LogWarning(error, "SignalR connection lost, attempting to reconnect...");
                OnConnectionStateChanged?.Invoke(HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += async connectionId =>
            {
                _logger.LogInformation("SignalR reconnected with connection ID: {ConnectionId}", connectionId);
                OnConnectionStateChanged?.Invoke(HubConnectionState.Connected);
                
                // Rejoin the dashboard group after reconnection
                if (!string.IsNullOrEmpty(_currentUserId))
                {
                    await JoinDashboardAsync(_currentUserId);
                }
            };

            _hubConnection.Closed += error =>
            {
                _logger.LogWarning(error, "SignalR connection closed");
                OnConnectionStateChanged?.Invoke(HubConnectionState.Disconnected);
                return Task.CompletedTask;
            };

            // Start connection
            await _hubConnection.StartAsync();
            _logger.LogInformation("SignalR connection started successfully");
            OnConnectionStateChanged?.Invoke(HubConnectionState.Connected);

            // Join the user's dashboard group
            await JoinDashboardAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection");
            throw;
        }
    }

    /// <summary>
    /// Join a user's dashboard group to receive their updates
    /// </summary>
    private async Task JoinDashboardAsync(string userId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("JoinDashboard", userId);
            _logger.LogInformation("Joined dashboard group for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Leave the current dashboard group
    /// </summary>
    public async Task LeaveDashboardAsync()
    {
        if (_hubConnection?.State == HubConnectionState.Connected && !string.IsNullOrEmpty(_currentUserId))
        {
            await _hubConnection.InvokeAsync("LeaveDashboard", _currentUserId);
            _logger.LogInformation("Left dashboard group for user {UserId}", _currentUserId);
        }
    }

    /// <summary>
    /// Stop the SignalR connection
    /// </summary>
    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await LeaveDashboardAsync();
            await _hubConnection.StopAsync();
            _logger.LogInformation("SignalR connection stopped");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await LeaveDashboardAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
            _logger.LogInformation("SignalR connection disposed");
        }
    }
}

