namespace DevMetricsPro.Application.DTOs.Auth;

/// <summary>
/// Response model for successful authentication
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's display name (if available)
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Token expiration time (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Optional: Refresh token for getting new access tokens
    /// </summary>
    public string? RefreshToken { get; set; }
}