using DevMetricsPro.Core.Entities;

namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Service for JWT token generation and management
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the authenticated user
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="roles">User's roles</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(ApplicationUser user, IList<string> roles);

    /// <summary>
    /// Generate a refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    Task<string> GenerateRefreshTokenAsync();
}