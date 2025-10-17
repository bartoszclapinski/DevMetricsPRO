namespace DevMetricsPro.Core.Enums;

/// <summary>
/// Types of metrics we track for developers
/// </summary>
public enum MetricType
{
    Commits,
    PullRequests,
    CodeReviews,
    IssuesClosed,
    LinesAdded,
    LinesRemoved,
    ActiveDays,
    AverageResponseTime
}