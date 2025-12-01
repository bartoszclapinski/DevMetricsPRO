namespace DevMetricsPro.Application.DTOs;

/// <summary>
/// Data transfer object for a single leaderboard entry
/// </summary>
public record LeaderboardEntryDto
{
    /// <summary>
    /// Current rank position (1-based)
    /// </summary>
    public int Rank { get; init; }

    /// <summary>
    /// Developer's unique identifier
    /// </summary>
    public Guid DeveloperId { get; init; }

    /// <summary>
    /// Developer's display name or username
    /// </summary>
    public required string DeveloperName { get; init; }

    /// <summary>
    /// Developer's avatar URL (optional)
    /// </summary>
    public string? AvatarUrl { get; init; }

    /// <summary>
    /// The metric value (e.g., commit count, PR count)
    /// </summary>
    public int Value { get; init; }

    /// <summary>
    /// Change indicator compared to previous period (e.g., "+5", "-2", "0")
    /// </summary>
    public string Change { get; init; } = "0";

    /// <summary>
    /// Whether the change is positive, negative, or neutral
    /// </summary>
    public TrendDirection Trend { get; init; } = TrendDirection.Neutral;
}

/// <summary>
/// Direction of trend change
/// </summary>
public enum TrendDirection
{
    Up,
    Down,
    Neutral
}

