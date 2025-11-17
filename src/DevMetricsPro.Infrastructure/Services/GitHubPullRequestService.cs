using System.Net.Http;
using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Infrastructure.Resilience;
using Microsoft.Extensions.Logging;
using Octokit;
using Polly.Retry;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for fetching pull request data from GitHub API using Octokit
/// </summary>
public class GitHubPullRequestService : IGitHubPullRequestService
{
    private readonly ILogger<GitHubPullRequestService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public GitHubPullRequestService(ILogger<GitHubPullRequestService> logger)
    {
        _logger = logger;
        _retryPolicy = GitHubResiliencePolicies.CreateRetryPolicy(logger, nameof(GitHubPullRequestService));
    }

    /// <summary>
    /// Fetches pull requests for a specific GitHub repository
    /// </summary>
    public async Task<IEnumerable<GitHubPullRequestDto>> GetRepositoryPullRequestsAsync(
        string owner,
        string repositoryName,
        string accessToken,
        DateTime? since = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async ct =>
            {
                ct.ThrowIfCancellationRequested();

                _logger.LogInformation(
                    "Fetching pull requests for {Owner}/{Repository} from GitHub API",
                    owner, repositoryName);

                var client = CreateClient(accessToken);

                var request = new PullRequestRequest
                {
                    State = ItemStateFilter.All,
                    SortProperty = PullRequestSort.Updated,
                    SortDirection = SortDirection.Descending
                };

                var pullRequests = await client.PullRequest.GetAllForRepository(owner, repositoryName, request);

                _logger.LogInformation(
                    "Successfully fetched {Count} pull requests for {Owner}/{Repository}",
                    pullRequests.Count, owner, repositoryName);

                var filteredPRs = since.HasValue
                    ? pullRequests.Where(pr => pr.UpdatedAt >= since.Value)
                    : pullRequests;

                var result = filteredPRs.Select(pr => new GitHubPullRequestDto
                {
                    Number = pr.Number,
                    Title = pr.Title,
                    State = pr.State.StringValue,
                    Body = pr.Body,
                    HtmlUrl = pr.HtmlUrl,
                    AuthorLogin = pr.User.Login,
                    AuthorName = pr.User.Name,
                    CreatedAt = pr.CreatedAt.UtcDateTime,
                    UpdatedAt = pr.UpdatedAt.UtcDateTime,
                    ClosedAt = pr.ClosedAt?.UtcDateTime,
                    MergedAt = pr.MergedAt?.UtcDateTime,
                    IsMerged = pr.Merged,
                    IsDraft = pr.Draft,
                    Additions = pr.Additions,
                    Deletions = pr.Deletions,
                    ChangedFiles = pr.ChangedFiles,
                    RepositoryName = repositoryName
                }).ToList();

                if (since.HasValue)
                {
                    _logger.LogInformation(
                        "Filtered to {Count} pull requests updated since {Since}",
                        result.Count, since.Value);
                }

                return result;
            }, cancellationToken);
        }
        catch (Octokit.NotFoundException ex)
        {
            _logger.LogError(ex,
                "Repository {Owner}/{Repository} not found on GitHub",
                owner, repositoryName);
            throw new DevMetricsPro.Core.Exceptions.NotFoundException(
                $"Repository {owner}/{repositoryName} not found.", ex);
        }
        catch (AuthorizationException ex)
        {
            _logger.LogError(ex,
                "GitHub authorization failed for {Owner}/{Repository}. Token may be invalid or expired",
                owner, repositoryName);
            throw new UnauthorizedAccessException(
                "GitHub authorization failed. Please reconnect your GitHub account.", ex);
        }
        catch (RateLimitExceededException ex)
        {
            _logger.LogWarning(ex, "GitHub API rate limit exceeded while fetching pull requests.");
            throw GitHubExceptionHelper.CreateRateLimitException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex,
                "GitHub API error fetching pull requests for {Owner}/{Repository}",
                owner, repositoryName);
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "GitHub is currently unavailable. Please try again shortly.",
                ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "Network error fetching pull requests for {Owner}/{Repository}",
                owner, repositoryName);
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "Unable to reach GitHub. Check your connection and try again.",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error fetching pull requests for {Owner}/{Repository}",
                owner, repositoryName);
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

