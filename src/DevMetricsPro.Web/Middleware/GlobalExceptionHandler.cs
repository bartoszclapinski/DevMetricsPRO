using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using DevMetricsPro.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Web;


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
        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var traceId = httpContext.TraceIdentifier;

        ProblemDetails problem;
        switch (exception)
        {
            case NotFoundException notFound:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problem = CreateProblem(
                    title: "Resource not found",
                    detail: notFound.Message,
                    status: StatusCodes.Status404NotFound,
                    traceId: traceId);
                break;
                
            case ValidationException validation:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problem = CreateProblem(
                    title: "Validation failed",
                    detail: validation.Message,
                    status: StatusCodes.Status400BadRequest,
                    traceId: traceId);
                break;

            case BusinessRuleException businessRule:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                problem = CreateProblem(
                    title: "Business rule violation",
                    detail: businessRule.Message,
                    status: StatusCodes.Status409Conflict,
                    traceId: traceId);
                break;

            case ExternalServiceException external:
                httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                problem = CreateProblem(
                    title: "External service unavailable",
                    detail: external.Message,
                    status: StatusCodes.Status503ServiceUnavailable,
                    traceId: traceId);
                break;
            
            case UnauthorizedAccessException unauthorized:
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problem = CreateProblem(
                    title: "Unauthorized",
                    detail: unauthorized.Message,
                    status: StatusCodes.Status401Unauthorized,
                    traceId: traceId);
                break;
            
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problem = CreateProblem(
                    title: "An unexpected error occurred",
                    detail: env.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                    status: StatusCodes.Status500InternalServerError,
                    traceId: traceId);
                break;
        }

        _logger.LogError(exception, "Error processing request {TraceId}", traceId);

        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true; // Exception was handled
    }

    private static ProblemDetails CreateProblem(string title, string detail, int status, string traceId)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = status,            
            Extensions = { ["traceId"] = traceId }
        };
    }
    
}