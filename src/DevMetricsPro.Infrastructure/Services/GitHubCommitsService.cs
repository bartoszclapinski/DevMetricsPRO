using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for fetching commit data from GitHub API using Octokit
/// </summary>
public class GitHubCommitsService : IGitHubCommitsService
{
    private readonly ILogger<GitHubCommitsService> _logger;

    public GitHubCommitsService(ILogger<GitHubCommitsService> logger)
    {
        _logger = logger;
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
            _logger.LogInformation(
                "Fetching commits from GitHub repository {Owner}/{Repo} since {Since}",
                owner, repositoryName, since?.ToString("yyyy-MM-dd") ?? "beginning");

            // Create GitHub client with OAuth token
            var client = new GitHubClient(new ProductHeaderValue("DevMetricsPro"))
            {
                Credentials = new Credentials(accessToken)
            };

            // Build request with optional since parameter
            var request = new CommitRequest();
            if (since.HasValue)
            {
                request.Since = new DateTimeOffset(since.Value);
            }

            // Fetch commits for the repository
            var commits = await client.Repository.Commit.GetAll(owner, repositoryName, request);

            _logger.LogInformation(
                "Successfully fetched {Count} commits from {Owner}/{Repo}",
                commits.Count, owner, repositoryName);

            // Map Octokit GitHubCommit objects to our DTO
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
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "Repository {Owner}/{Repo} not found", owner, repositoryName);
            throw new InvalidOperationException($"Repository {owner}/{repositoryName} not found.", ex);
        }
        catch (AuthorizationException ex)
        {
            _logger.LogError(ex, "GitHub authorization failed. Token may be invalid or expired");
            throw new UnauthorizedAccessException("GitHub authorization failed. Please reconnect your GitHub account.", ex);
        }
        catch (RateLimitExceededException ex)
        {
            _logger.LogError(ex, "GitHub API rate limit exceeded");
            throw new InvalidOperationException("GitHub API rate limit exceeded. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching commits from {Owner}/{Repo}", owner, repositoryName);
            throw;
        }
    }
}