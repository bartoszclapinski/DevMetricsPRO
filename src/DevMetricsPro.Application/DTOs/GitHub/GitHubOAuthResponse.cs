namespace DevMetricsPro.Application.DTOs.GitHub;

/// <summary>
/// Response after successful GitHub OAuth authentication
/// </summary>
public class GitHubOAuthResponse
{
    /// <summary>
    /// GitHub access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token type (usually bearer)
    /// </summary>
    public string TokenType { get; set; } = "bearer";

    /// <summary>
    /// Granted scopes
    /// </summary>
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// GitHub username
    /// </summary>
    public string? GitHubUsername { get; set; }

    /// <summary>
    /// GitHub user ID
    /// </summary>
    public long? GitHubUserId { get; set; }

    /// <summary>
    /// User's email from GitHub
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// When the connection was established
    /// </summary>
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

}