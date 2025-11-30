namespace DevMetricsPro.Application.DTOs.Charts;

/// <summary>
/// DTO for GitHub-style contribution heatmap data.
/// </summary>
public record ContributionHeatmapDto
{
    /// <summary>
    /// List of daily contributions for the heatmap.
    /// </summary>
    public required List<DayContribution> Days { get; init; }
    
    /// <summary>
    /// Maximum contributions on a single day (for scaling colors).
    /// </summary>
    public int MaxContributions { get; init; }
    
    /// <summary>
    /// Total contributions in the period.
    /// </summary>
    public int TotalContributions { get; init; }
    
    /// <summary>
    /// Number of weeks displayed.
    /// </summary>
    public int NumberOfWeeks { get; init; }
    
    /// <summary>
    /// Start date of the heatmap.
    /// </summary>
    public DateTime StartDate { get; init; }
    
    /// <summary>
    /// End date of the heatmap.
    /// </summary>
    public DateTime EndDate { get; init; }
    
    /// <summary>
    /// Creates an empty heatmap for the specified period.
    /// </summary>
    public static ContributionHeatmapDto CreateEmpty(int numberOfWeeks)
    {
        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate.AddDays(-(numberOfWeeks * 7) + 1);
        
        var days = new List<DayContribution>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            days.Add(new DayContribution
            {
                Date = date,
                Count = 0,
                Level = ContributionLevel.None
            });
        }
        
        return new ContributionHeatmapDto
        {
            Days = days,
            MaxContributions = 0,
            TotalContributions = 0,
            NumberOfWeeks = numberOfWeeks,
            StartDate = startDate,
            EndDate = endDate
        };
    }
}

/// <summary>
/// Represents a single day's contribution data.
/// </summary>
public record DayContribution
{
    /// <summary>
    /// The date of this contribution.
    /// </summary>
    public DateTime Date { get; init; }
    
    /// <summary>
    /// Number of contributions (commits) on this day.
    /// </summary>
    public int Count { get; init; }
    
    /// <summary>
    /// Contribution level for color coding.
    /// </summary>
    public ContributionLevel Level { get; init; }
}

/// <summary>
/// Contribution intensity levels for heatmap coloring.
/// </summary>
public enum ContributionLevel
{
    /// <summary>No contributions (0)</summary>
    None = 0,
    
    /// <summary>Low activity (1-2 commits)</summary>
    Low = 1,
    
    /// <summary>Medium activity (3-5 commits)</summary>
    Medium = 2,
    
    /// <summary>High activity (6-9 commits)</summary>
    High = 3,
    
    /// <summary>Maximum activity (10+ commits)</summary>
    Max = 4
}

