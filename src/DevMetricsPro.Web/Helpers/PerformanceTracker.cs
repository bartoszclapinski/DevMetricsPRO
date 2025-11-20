using System.Diagnostics;

namespace DevMetricsPro.Web.Helpers;

/// <summary>
/// Helper for tracking and logging performance of operations
/// </summary>
public sealed class PerformanceTracker : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;
    private readonly Dictionary<string, object> _context;
    private readonly int _warningThresholdMs;

    /// <summary>
    /// Creates a new performance tracker
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="operationName">Name of the operation being tracked</param>
    /// <param name="warningThresholdMs">Threshold in milliseconds to log a warning</param>
    /// <param name="context">Additional context to include in logs</param>
    public PerformanceTracker(
        ILogger logger, 
        string operationName, 
        int warningThresholdMs = 1000,
        Dictionary<string, object>? context = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _operationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
        _warningThresholdMs = warningThresholdMs;
        _context = context ?? new Dictionary<string, object>();
        _stopwatch = Stopwatch.StartNew();

        _logger.LogDebug("Starting operation: {OperationName}", _operationName);
    }

    /// <summary>
    /// Adds context information to be included in the completion log
    /// </summary>
    public void AddContext(string key, object value)
    {
        _context[key] = value;
    }

    /// <summary>
    /// Completes the operation and logs the duration
    /// </summary>
    public void Dispose()
    {
        _stopwatch.Stop();
        var elapsedMs = _stopwatch.ElapsedMilliseconds;

        var contextString = _context.Any() 
            ? string.Join(", ", _context.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            : "no additional context";

        if (elapsedMs >= _warningThresholdMs)
        {
            _logger.LogWarning(
                "SLOW OPERATION: {OperationName} completed in {ElapsedMs}ms. Context: {Context}",
                _operationName,
                elapsedMs,
                contextString);
        }
        else
        {
            _logger.LogInformation(
                "Operation {OperationName} completed in {ElapsedMs}ms",
                _operationName,
                elapsedMs);
        }
    }

    /// <summary>
    /// Creates a performance tracker for database operations (1 second threshold)
    /// </summary>
    public static PerformanceTracker ForDatabaseOperation(ILogger logger, string operationName, Dictionary<string, object>? context = null)
    {
        return new PerformanceTracker(logger, $"DB: {operationName}", 1000, context);
    }

    /// <summary>
    /// Creates a performance tracker for external API calls (3 second threshold)
    /// </summary>
    public static PerformanceTracker ForExternalApi(ILogger logger, string operationName, Dictionary<string, object>? context = null)
    {
        return new PerformanceTracker(logger, $"API: {operationName}", 3000, context);
    }

    /// <summary>
    /// Creates a performance tracker for background jobs (30 second threshold)
    /// </summary>
    public static PerformanceTracker ForBackgroundJob(ILogger logger, string jobName, Dictionary<string, object>? context = null)
    {
        return new PerformanceTracker(logger, $"Job: {jobName}", 30000, context);
    }
}

