using System.Diagnostics;

namespace DevMetricsPro.Web.Middleware;

/// <summary>
/// Middleware that logs performance metrics for slow requests
/// </summary>
public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceLoggingMiddleware> _logger;
    
    // Performance thresholds in milliseconds
    private const int WarningThresholdMs = 1000;  // 1 second
    private const int CriticalThresholdMs = 3000; // 3 seconds

    public PerformanceLoggingMiddleware(RequestDelegate next, ILogger<PerformanceLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Log slow requests
            if (elapsedMs >= CriticalThresholdMs)
            {
                _logger.LogWarning(
                    "SLOW REQUEST: {Method} {Path} completed in {ElapsedMs}ms (Status: {StatusCode})",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs,
                    context.Response.StatusCode);
            }
            else if (elapsedMs >= WarningThresholdMs)
            {
                _logger.LogInformation(
                    "Request {Method} {Path} took {ElapsedMs}ms (Status: {StatusCode})",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs,
                    context.Response.StatusCode);
            }
            else
            {
                // Debug level for normal requests
                _logger.LogDebug(
                    "Request {Method} {Path} completed in {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs);
            }
        }
    }
}

/// <summary>
/// Extension methods for registering PerformanceLoggingMiddleware
/// </summary>
public static class PerformanceLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds performance logging middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UsePerformanceLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceLoggingMiddleware>();
    }
}

