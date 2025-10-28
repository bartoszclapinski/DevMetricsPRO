namespace DevMetricsPro.Application.DTOs.GitHub;

/// <summary>
/// Request to initiate GitHub OAuth flow
/// </summary>
public class GitHubOAuthRequest
{
    /// <summary>
    /// Redirect URI after GitHub authorization
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// OAuth scopes requested (e.g. "repo,read:user")
    /// </summary>
    public string Scope { get; set; } = "repo,read:user";
}
