using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace DevMetricsPro.Web.Services;

/// <summary>
/// Service for managing authentication state
/// </summary>
public class AuthStateService
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public AuthStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Get the authentication token from local storage
    /// </summary>
    /// <returns>The authentication token or null if not found</returns>
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey); 
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Save the authentication token to local storage
    /// </summary>
    public async Task SaveTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }

    /// <summary>
    /// Remove the authentication token from local storage
    /// </summary>
    public async Task RemoveTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }

    /// <summary>
    /// Check if the user is authenticated
    /// </summary>
    /// <returns>True if the user is authenticated, false otherwise</returns>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        try
        {
            // Check if token is expired
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the user information from the JWT token in local storage
    /// </summary>
    /// <returns>The user information or null if not found</returns>
    public async Task<UserInfo?> GetUserInfoAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return null;

        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return new UserInfo
            {
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                DisplayName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty,
                Roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
            };
        }
        catch 
        {
            return null;
        }
    }

    /// <summary>
    /// Get the user ID from the JWT token
    /// </summary>
    /// <returns>The user ID or null if not found</returns>
    public async Task<string?> GetUserIdAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return null;

        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            // Try standard claims first, then JWT-specific ones
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        }
        catch
        {
            return null;
        }
    }
}

public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}