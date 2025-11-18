namespace DevMetricsPro.Application.Caching;

/// <summary>
/// TTL definitions for frequently cached data.
/// </summary>
public static class CacheDurations
{
    public static readonly TimeSpan GitHubConnectionStatus = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan GitHubRepositories = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan DashboardMetrics = TimeSpan.FromMinutes(10);
}

