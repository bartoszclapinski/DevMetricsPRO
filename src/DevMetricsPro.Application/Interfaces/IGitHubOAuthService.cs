using DevMetricsPro.Application.DTOs.GitHub;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for handling Github OAuth authentication flow
/// </summary>
public interface IGitHubOAuthService
{
    /// <summary>
    /// Generates the GitHub OAuth authorization URL
    /// </summary>
    /// <param name="state">CSRF protection state parameter</param>
    /// <returns>Authorization URL to redirect user to</returns>
    string GetAuthorizationUrl(string state);

    /// <summary>
    /// Exchanges authorization code for access token
    /// </summary>
    /// <param name="code">Authorization code from GitHub</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OAuth response with access token and user info</returns>
    Task<GitHubOAuthResponse> ExchangeCodeForTokenAsync(
        string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets GitHub user information using access token
    /// </summary>
    /// <param name="accessToken">GitHub access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information from GitHub</returns>
    Task<GitHubOAuthResponse> GetUserInfoAsync(
        string accessToken, CancellationToken cancellationToken = default);
       
}