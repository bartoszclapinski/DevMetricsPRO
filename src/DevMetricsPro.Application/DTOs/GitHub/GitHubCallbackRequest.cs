namespace DevMetricsPro.Application.DTOs.GitHub;

/// <summary>
/// Request received from GitHub OAuth callback
/// </summary>
public class GitHubCallbackRequest
{
    /// <summary>
    /// Authorization code from GitHub
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// State parameter for CSRF protection
    /// </summary>
    public string? State { get; set; }
}