using System.Net.Http;
using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Infrastructure.Resilience;
using Microsoft.Extensions.Logging;
using Octokit;
using Polly.Retry;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for fetching repository data from GitHub API using Octokit
/// </summary>
public class GitHubRepositoryService : IGitHubRepositoryService
{
    private readonly ILogger<GitHubRepositoryService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public GitHubRepositoryService(ILogger<GitHubRepositoryService> logger)
    {
        _logger = logger;
        _retryPolicy = GitHubResiliencePolicies.CreateRetryPolicy(logger, nameof(GitHubRepositoryService));
    }

    /// <summary>
    /// Fetches all repositories for the authenticated GitHub user
    /// </summary>    
    public async Task<IEnumerable<GitHubRepositoryDto>> GetUserRepositoriesAsync(
        string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async ct =>
            {
                ct.ThrowIfCancellationRequested();

                _logger.LogInformation("Fetching repositories from GitHub API");

                var client = CreateClient(accessToken);

                var repositories = await client.Repository.GetAllForCurrent();

                _logger.LogInformation("Successfully fetched {Count} repositories from GitHub", repositories.Count);

                return repositories.Select(repo => new GitHubRepositoryDto
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Description = repo.Description,
                    HtmlUrl = repo.HtmlUrl,
                    FullName = repo.FullName,
                    IsPrivate = repo.Private,
                    IsFork = repo.Fork,
                    StargazersCount = repo.StargazersCount,
                    ForksCount = repo.ForksCount,
                    OpenIssuesCount = repo.OpenIssuesCount,
                    Language = repo.Language,
                    CreatedAt = repo.CreatedAt.UtcDateTime,
                    UpdatedAt = repo.UpdatedAt.UtcDateTime,
                    PushedAt = repo.PushedAt?.UtcDateTime
                });
            }, cancellationToken);
        }
        catch (AuthorizationException ex)
        {
            _logger.LogError(ex, "GitHub authorization failed. Token may be invalid or expired");
            throw new UnauthorizedAccessException("GitHub authorization failed. Please reconnect your GitHub account.", ex);
        }
        catch (RateLimitExceededException ex)
        {
            _logger.LogWarning(ex, "GitHub API rate limit exceeded.");
            throw GitHubExceptionHelper.CreateRateLimitException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "GitHub API error while fetching repositories.");
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "GitHub is currently unavailable. Please try again shortly.",
                ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while fetching repositories from GitHub.");
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "Unable to reach GitHub. Check your connection and try again.",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories from GitHub");
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "Unexpected error while contacting GitHub.", ex);
        }
    }

    private static GitHubClient CreateClient(string accessToken) =>
        new(new ProductHeaderValue("DevMetricsPro"))
        {
            Credentials = new Credentials(accessToken)
        };
}