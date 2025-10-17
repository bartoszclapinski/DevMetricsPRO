using DevMetricsPro.Core.Enums;

namespace DevMetricsPro.Core.Entities;

/// <summary>
/// Represents a calculated metric for a developer
/// </summary>
public class Metric : BaseEntity
{

    /// <summary>
    /// Foreign key to the developer
    /// </summary>
    public Guid DeveloperId { get; set; }

    /// <summary>
    /// Foreign key to the repository (optional - can be a global metric)
    /// </summary>
    public Guid? RepositoryId { get; set; }

    /// <summary>
    /// Type of metric being tracked
    /// </summary>
    public MetricType Type { get; set; }

    /// <summary>
    /// The metric value (e.g. number of commits, hours, percentage)
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// When this metric was recorded
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Additional metadata in JSON format (optional)
    /// For example: { "branch": "main", "project": "backend"}
    /// </summary>
    public string? Metadata { get; set; }


    // Navigation properties

    /// <summary>
    /// The developer this metric belongs to
    /// </summary>
    public Developer Developer { get; set; } = null!;

    /// <summary>
    /// The repository this metric is for (optional)
    /// </summary>
    public Repository? Repository { get; set; }
}