using System.Net.Http;
using Microsoft.Extensions.Logging;
using Octokit;
using Polly;
using Polly.Retry;

namespace DevMetricsPro.Infrastructure.Resilience;

internal static class GitHubResiliencePolicies
{

    public static AsyncRetryPolicy CreateRetryPolicy(ILogger logger, string operationName)
    {
        return Policy
            .Handle<ApiException>(ex => ex is not RateLimitExceededException)
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                {
                    var exponentialBackoff = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(100, 500));
                    return exponentialBackoff + jitter;
                },
                onRetry: (exception, delay, attempt, context) =>
                {
                    logger.LogWarning(
                        exception,
                        "Transient GitHub error during {Operation}. Retrying attempt {Attempt} in {Delay}.",
                        operationName,
                        attempt,
                        delay);
                });
    }
}

