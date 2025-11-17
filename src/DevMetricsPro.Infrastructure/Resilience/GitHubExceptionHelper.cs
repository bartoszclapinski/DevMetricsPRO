using DevMetricsPro.Core.Exceptions;
using Octokit;

namespace DevMetricsPro.Infrastructure.Resilience;

internal static class GitHubExceptionHelper
{
    public static ExternalServiceException CreateRateLimitException(RateLimitExceededException exception)
    {
        var resetUtc = exception.Reset.UtcDateTime;
        var waitTime = resetUtc - DateTime.UtcNow;
        if (waitTime < TimeSpan.Zero)
        {
            waitTime = TimeSpan.Zero;
        }

        var waitDescription = waitTime == TimeSpan.Zero
            ? "Please retry shortly."
            : $"Please retry in approximately {Math.Ceiling(waitTime.TotalMinutes)} minute(s).";

        var message = $"GitHub API rate limit exceeded. {waitDescription}";
        return new ExternalServiceException(message, exception);
    }

    public static ExternalServiceException CreateExternalServiceException(string message, Exception exception) =>
        new(message, exception);
}

