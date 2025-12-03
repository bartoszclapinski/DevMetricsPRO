namespace DevMetricsPro.Web.Services;

/// <summary>
/// Manages global dashboard state including time range filters.
/// Registered as Scoped service so state persists within user session.
/// </summary>
public class DashboardStateService : IDisposable
{
    private DateTime _startDate;
    private DateTime _endDate;
    private TimeRangePreset _selectedPreset;
    private bool _disposed;

    /// <summary>
    /// Event fired when dashboard state changes. Components should subscribe to this.
    /// </summary>
    public event Action? OnStateChanged;

    public DashboardStateService()
    {
        // Default to 30 days
        _selectedPreset = TimeRangePreset.Last30Days;
        _endDate = DateTime.UtcNow.Date;
        _startDate = _endDate.AddDays(-30);
    }

    /// <summary>
    /// Current start date for filtering
    /// </summary>
    public DateTime StartDate => _startDate;

    /// <summary>
    /// Current end date for filtering
    /// </summary>
    public DateTime EndDate => _endDate;

    /// <summary>
    /// Currently selected preset
    /// </summary>
    public TimeRangePreset SelectedPreset => _selectedPreset;

    /// <summary>
    /// Sets the time range using a preset
    /// </summary>
    public void SetTimeRange(TimeRangePreset preset)
    {
        _selectedPreset = preset;
        _endDate = DateTime.UtcNow.Date;

        _startDate = preset switch
        {
            TimeRangePreset.Last7Days => _endDate.AddDays(-7),
            TimeRangePreset.Last30Days => _endDate.AddDays(-30),
            TimeRangePreset.Last90Days => _endDate.AddDays(-90),
            TimeRangePreset.Last365Days => _endDate.AddDays(-365),
            TimeRangePreset.AllTime => DateTime.MinValue,
            TimeRangePreset.Custom => _startDate, // Keep existing start date for custom
            _ => _endDate.AddDays(-30)
        };

        NotifyStateChanged();
    }

    /// <summary>
    /// Sets a custom date range
    /// </summary>
    public void SetCustomRange(DateTime startDate, DateTime endDate)
    {
        _selectedPreset = TimeRangePreset.Custom;
        _startDate = startDate.Date;
        _endDate = endDate.Date;

        NotifyStateChanged();
    }

    /// <summary>
    /// Gets the number of days in the current range
    /// </summary>
    public int GetDaysInRange()
    {
        if (_selectedPreset == TimeRangePreset.AllTime)
            return int.MaxValue;

        return (int)(_endDate - _startDate).TotalDays;
    }

    /// <summary>
    /// Gets user-friendly label for current range
    /// </summary>
    public string GetRangeLabel()
    {
        return _selectedPreset switch
        {
            TimeRangePreset.Last7Days => "Last 7 Days",
            TimeRangePreset.Last30Days => "Last 30 Days",
            TimeRangePreset.Last90Days => "Last 90 Days",
            TimeRangePreset.Last365Days => "Last Year",
            TimeRangePreset.AllTime => "All Time",
            TimeRangePreset.Custom => $"{_startDate:MMM d} - {_endDate:MMM d, yyyy}",
            _ => "Last 30 Days"
        };
    }

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            OnStateChanged = null;
            _disposed = true;
        }
    }
}

/// <summary>
/// Preset time ranges for quick selection
/// </summary>
public enum TimeRangePreset
{
    Last7Days,
    Last30Days,
    Last90Days,
    Last365Days,
    AllTime,
    Custom
}

