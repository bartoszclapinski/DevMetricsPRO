using System.Net.Http;
using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Infrastructure.Resilience;
using Microsoft.Extensions.Logging;
using Octokit;
using Polly.Retry;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for fetching commit data from GitHub API using Octokit
/// </summary>
public class GitHubCommitsService : IGitHubCommitsService
{
    private readonly ILogger<GitHubCommitsService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public GitHubCommitsService(ILogger<GitHubCommitsService> logger)
    {
        _logger = logger;
        _retryPolicy = GitHubResiliencePolicies.CreateRetryPolicy(logger, nameof(GitHubCommitsService));
    }

    /// <summary>
    /// Fetches commits for a specific repository
    /// </summary>
    public async Task<IEnumerable<GitHubCommitDto>> GetRepositoryCommitsAsync(
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
                    "Fetching commits from GitHub repository {Owner}/{Repo} since {Since}",
                    owner, repositoryName, since?.ToString("yyyy-MM-dd") ?? "beginning");

                var client = CreateClient(accessToken);

                var request = new CommitRequest();
                if (since.HasValue)
                {
                    request.Since = new DateTimeOffset(since.Value);
                }

                var commits = await client.Repository.Commit.GetAll(owner, repositoryName, request);

                _logger.LogInformation(
                    "Successfully fetched {Count} commits from {Owner}/{Repo}",
                    commits.Count, owner, repositoryName);

                return commits.Select(commit => new GitHubCommitDto
                {
                    Sha = commit.Sha,
                    Message = commit.Commit.Message,
                    AuthorName = commit.Commit.Author.Name,
                    AuthorEmail = commit.Commit.Author.Email,
                    AuthorDate = commit.Commit.Author.Date.UtcDateTime,
                    CommitterName = commit.Commit.Committer.Name,
                    CommitterEmail = commit.Commit.Committer.Email,
                    CommitterDate = commit.Commit.Committer.Date.UtcDateTime,
                    HtmlUrl = commit.HtmlUrl,
                    Additions = commit.Stats?.Additions ?? 0,
                    Deletions = commit.Stats?.Deletions ?? 0,
                    TotalChanges = commit.Stats?.Total ?? 0,
                    RepositoryName = repositoryName
                });
            }, cancellationToken);
        }
        catch (Octokit.NotFoundException ex)
        {
            _logger.LogError(ex, "Repository {Owner}/{Repo} not found", owner, repositoryName);
            throw new DevMetricsPro.Core.Exceptions.NotFoundException($"Repository {owner}/{repositoryName} not found.", ex);
        }
        catch (AuthorizationException ex)
        {
            _logger.LogError(ex, "GitHub authorization failed. Token may be invalid or expired");
            throw new UnauthorizedAccessException("GitHub authorization failed. Please reconnect your GitHub account.", ex);
        }
        catch (RateLimitExceededException ex)
        {
            _logger.LogWarning(ex, "GitHub API rate limit exceeded while fetching commits.");
            throw GitHubExceptionHelper.CreateRateLimitException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "GitHub API error fetching commits for {Owner}/{Repo}", owner, repositoryName);
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "GitHub is currently unavailable. Please try again shortly.",
                ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error fetching commits for {Owner}/{Repo}", owner, repositoryName);
            throw GitHubExceptionHelper.CreateExternalServiceException(
                "Unable to reach GitHub. Check your connection and try again.",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching commits from {Owner}/{Repo}", owner, repositoryName);
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