using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Service for fetching repository data from GitHub API using Octokit
/// </summary>
public class GitHubRepositoryService : IGitHubRepositoryService
{
    private readonly ILogger<GitHubRepositoryService> _logger;

    public GitHubRepositoryService(ILogger<GitHubRepositoryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Fetches all repositories for the authenticated GitHub user
    /// </summary>    
    public async Task<IEnumerable<GitHubRepositoryDto>> GetUserRepositoriesAsync(
        string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching repositories from GitHub API");
            
            // Create GitHub client with OAuth token
            var client = new GitHubClient(new ProductHeaderValue("DevMetricsPro"))
            {
                Credentials = new Credentials(accessToken)
            };

            // Fetch all repositories for the authenticated user
            var repositories = await client.Repository.GetAllForCurrent();

            _logger.LogInformation("Successfully fetched {Count} repositories from GitHub", repositories.Count);

            // Map Octokit Repository objects to our DTO
            return repositories.Select(repo => new GitHubRepositoryDto
            {
                Id = repo.Id,
                Name = repo.Name,
                Description = repo.Description,
                HtmlUrl = repo.HtmlUrl,
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
        }
        catch (AuthorizationException ex)
        {
            _logger.LogError(ex, "GitHub authorization failed. Token may be invalid or expired");
            throw new UnauthorizedAccessException("GitHub authorization failed. Please reconnect your GitHub account.", ex);
        }
        catch (RateLimitExceededException ex)
        {
            _logger.LogError(ex, "GitHub API rate limit exceeded.");
            throw new InvalidOperationException("GitHub API rate limit exceeded. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories from GitHub");
            throw;
        }
    }
}