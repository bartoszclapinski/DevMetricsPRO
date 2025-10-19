using System.Net;
using Microsoft.AspNetCore.Diagnostics;


namespace DevMetricsPro.Web.Middleware;

/// <summary>
/// Global exception handler middlware that catches unhandled exceptions
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles unhandled exceptions and returns appropriate error response
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhadled exception occured: {Message}", exception.Message);

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = "An error occured processing your request",
            message = httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                ? exception.Message
                : "An unexpected error occured.",
            type = exception.GetType().Name,
            path = httpContext.Request.Path.Value
        };

        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true; // Exception was handled
    }
    
}