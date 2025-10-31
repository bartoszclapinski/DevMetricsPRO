using DevMetricsPro.Application.DTOs.GitHub;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for fetching commits from GitHub API
/// </summary>
public interface IGitHubCommitsService
{
    /// <summary>
    /// Fetches commits for a specific repository
    /// </summary>
    /// <param name="owner">The owner of the repository (username or organization name)</param>
    /// <param name="repositoryName">The name of the repository</param>
    /// <param name="accessToken">GitHub OAuth access token</param>
    /// <param name="since">Only fetch commits since this date (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of commits</returns>
    Task<IEnumerable<GitHubCommitDto>> GetRepositoryCommitsAsync(
        string owner, 
        string repositoryName, 
        string accessToken, 
        DateTime? since = null, 
        CancellationToken cancellationToken = default);
}