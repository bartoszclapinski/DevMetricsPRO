namespace DevMetricsPro.Application.Caching;

/// <summary>
/// Central place for cache key naming conventions to avoid typos.
/// </summary>
public static class CacheKeys
{
    private const string Prefix = "devmetrics";

    public static string GitHubConnectionStatus(Guid userId) =>
        $"{Prefix}:github:status:{userId}";

    public static string GitHubRepositories(Guid userId) =>
        $"{Prefix}:github:repos:{userId}";

    public static string GitHubRecentCommits(Guid userId, int limit) =>
        $"{Prefix}:github:commits:{userId}:limit:{limit}";
}

