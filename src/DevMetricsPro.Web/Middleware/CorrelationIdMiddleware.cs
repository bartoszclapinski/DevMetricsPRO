using System.Diagnostics;

namespace DevMetricsPro.Web.Middleware;

/// <summary>
/// Middleware that adds a correlation ID to each request for distributed tracing
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation ID from request header, or generate a new one
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault()
                            ?? context.TraceIdentifier;

        // Store in HttpContext for use throughout the request
        context.Items["CorrelationId"] = correlationId;

        // Add to response headers for client tracking
        context.Response.Headers.TryAdd(CorrelationIdHeaderName, correlationId);

        // Add to log scope so all logs in this request include the correlation ID
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method
        }))
        {
            await _next(context);
        }
    }
}

/// <summary>
/// Extension methods for registering CorrelationIdMiddleware
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    /// <summary>
    /// Adds correlation ID middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}

