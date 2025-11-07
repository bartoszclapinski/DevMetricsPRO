using DevMetricsPro.Application.DTOs.GitHub;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service interface for fetching pull request data from GitHub API
/// </summary>
public interface IGitHubPullRequestService
{
    /// <summary>
    /// Fetches pull requests for a specific GitHub repository
    /// </summary>
    /// <param name="owner">Repository owner (username or organization)</param>
    /// <param name="repositoryName">Repository name</param>
    /// <param name="accessToken">GitHub OAuth access token</param>
    /// <param name="since">Optional date to fetch PRs updated after this date (for incremental sync)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of pull request DTOs</returns>
    Task<IEnumerable<GitHubPullRequestDto>> GetRepositoryPullRequestsAsync(
        string owner,
        string repositoryName,
        string accessToken,
        DateTime? since = null,
        CancellationToken cancellationToken = default);
}

