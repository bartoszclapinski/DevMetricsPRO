using DevMetricsPro.Application.DTOs.GitHub;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for interacting with GitHub repositories API
/// </summary>
public interface IGitHubRepositoryService
{
    /// <summary>
    /// Fetches all repositories for GitHub user
    /// </summary>
    /// <param name="accessToken">GitHub OAuth access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of repository information</returns>
    Task<IEnumerable<GitHubRepositoryDto>> GetUserRepositoriesAsync(
        string accessToken, CancellationToken cancellationToken = default
    );
}