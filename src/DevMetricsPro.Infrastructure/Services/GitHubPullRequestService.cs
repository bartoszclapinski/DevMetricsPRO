using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for fetching pull request data from GitHub API using Octokit
/// </summary>
public class GitHubPullRequestService : IGitHubPullRequestService
{
    private readonly ILogger<GitHubPullRequestService> _logger;

    public GitHubPullRequestService(ILogger<GitHubPullRequestService> logger)
    {
        _logger = logger;
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
            _logger.LogInformation(
                "Fetching pull requests for {Owner}/{Repository} from GitHub API",
                owner, repositoryName);

            // Create GitHub client with OAuth token
            var client = new GitHubClient(new ProductHeaderValue("DevMetricsPro"))
            {
                Credentials = new Credentials(accessToken)
            };

            // Create PR request with filters
            var request = new PullRequestRequest
            {
                State = ItemStateFilter.All, // Fetch both open and closed PRs
                SortProperty = PullRequestSort.Updated,
                SortDirection = SortDirection.Descending
            };

            // Fetch all pull requests
            var pullRequests = await client.PullRequest.GetAllForRepository(owner, repositoryName, request);

            _logger.LogInformation(
                "Successfully fetched {Count} pull requests for {Owner}/{Repository}",
                pullRequests.Count, owner, repositoryName);

            // Filter by date if 'since' is provided (for incremental sync)
            var filteredPRs = since.HasValue
                ? pullRequests.Where(pr => pr.UpdatedAt >= since.Value)
                : pullRequests;

            // Map Octokit PullRequest objects to our DTO
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
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex,
                "Repository {Owner}/{Repository} not found on GitHub",
                owner, repositoryName);
            throw new InvalidOperationException(
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
            _logger.LogError(ex, "GitHub API rate limit exceeded");
            throw new InvalidOperationException(
                "GitHub API rate limit exceeded. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error fetching pull requests for {Owner}/{Repository}",
                owner, repositoryName);
            throw;
        }
    }
}

