using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace DevMetricsPro.Web.Configuration;

/// <summary>
/// Configuration for API rate limiting policies
/// </summary>
public static class RateLimitingConfiguration
{
    public const string ApiPolicy = "ApiRateLimit";
    public const string AuthPolicy = "AuthRateLimit";
    public const string SyncPolicy = "SyncRateLimit";

    /// <summary>
    /// Configures rate limiting policies for the application
    /// </summary>
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global rate limiter - reject requests when limits are exceeded
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // API endpoints: 100 requests per minute per IP
            options.AddFixedWindowLimiter(ApiPolicy, limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // Auth endpoints: 5 login attempts per minute per IP (prevent brute force)
            options.AddFixedWindowLimiter(AuthPolicy, limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0; // No queueing for auth
            });

            // Sync endpoints: 10 requests per hour per user (expensive operations)
            options.AddFixedWindowLimiter(SyncPolicy, limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromHours(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 2;
            });

            // Global fallback: 1000 requests per minute per IP
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1000,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 50
                    });
            });

            // Custom response when rate limit is exceeded
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                
                TimeSpan? retryAfter = null;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue))
                {
                    retryAfter = retryAfterValue;
                    context.HttpContext.Response.Headers.RetryAfter = retryAfterValue.TotalSeconds.ToString();
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Rate limit exceeded",
                    message = "Too many requests. Please try again later.",
                    retryAfterSeconds = retryAfter?.TotalSeconds
                }, cancellationToken);
            };
        });

        return services;
    }
}

