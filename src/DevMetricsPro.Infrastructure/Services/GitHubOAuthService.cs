using System.Net.Http.Json;
using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace DevMetricsPro.Infrastructure.Services;

/// <summary>
/// Implementation of GitHub OAuth authentication service
/// </summary>
public class GitHubOAuthService : IGitHubOAuthService 
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GitHubOAuthService> _logger;

    private const string GitHubAuthUrl = "https://github.com/login/oauth/authorize";
    private const string GitHubTokenUrl = "https://github.com/login/oauth/access_token";
    private const string GitHubApiUrl = "https://api.github.com/user";

    public GitHubOAuthService(
        IConfiguration configuration, 
        IHttpClientFactory httpClientFactory, 
        ILogger<GitHubOAuthService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Generates the GitHub OAuth authorization URL
    /// </summary>
    public string GetAuthorizationUrl(string state)
    {
        var clientId = _configuration["GitHub:ClientId"];
        var redirectUri = _configuration["GitHub:RedirectUri"];
        var scopes = _configuration["GitHub:Scopes"];

        // Build authorization URL
        var url = $"{GitHubAuthUrl}?" +
                  $"client_id={clientId}&" +
                  $"redirect_uri={Uri.EscapeDataString(redirectUri!)}&" +
                  $"scope={Uri.EscapeDataString(scopes!)}&" +
                  $"state={state}";

        _logger.LogInformation("Generated GitHub OAuth URL for state: {State}", state);

        return url;
    }

    /// <summary>
    /// Exchanges authorization code for access token
    /// </summary>
    public async Task<GitHubOAuthResponse> ExchangeCodeForTokenAsync(
        string code, CancellationToken cancellationToken = default)
    {
        var clientId = _configuration["GitHub:ClientId"];
        var clientSecret = _configuration["GitHub:ClientSecret"];
        var redirectUri = _configuration["GitHub:RedirectUri"];

        // Prepare token exchange request

        var requestData = new Dictionary<string, string>
        {
            {"client_id", clientId!},
            {"client_secret", clientSecret!},
            {"code", code},
            {"redirect_uri", redirectUri!}
        };

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        try
        {
            // Exchange code for token (GitHub requires form-encoded, not JSON)
            var content = new FormUrlEncodedContent(requestData);
            var response = await client.PostAsync(
                GitHubTokenUrl,
                content,
                cancellationToken   
            );

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<GitHubTokenResponse>(
                cancellationToken: cancellationToken);

            // Add better logging
            _logger.LogInformation(
                "GitHub token response: AccessToken={HasToken}, TokenType={TokenType}, Scope={Scope}",
                !string.IsNullOrEmpty(tokenResponse?.AccessToken),
                tokenResponse?.TokenType,
                tokenResponse?.Scope
            );    

            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("Failed to get access token from GitHub");
                throw new InvalidOperationException("Failed to obtain access token from GitHub");
            }

            _logger.LogInformation("Successfully exchanged code for GitHub access token");

            // Get user info with the token
            return await GetUserInfoAsync(tokenResponse.AccessToken, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during GitHub token exchange");
            throw;
        }
    }

    /// <summary>
    /// Gets GitHub user information using access token
    /// </summary>
    public async Task<GitHubOAuthResponse> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        client.DefaultRequestHeaders.Add("User-Agent", "DevMetricsPro");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

        try
        {
            // Get user info from GitHub API
            var response = await client.GetAsync(GitHubApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var userInfo = await response.Content.ReadFromJsonAsync<GitHubUserInfo>(
                cancellationToken: cancellationToken);
            
            if (userInfo == null)
            {
                _logger.LogError("Failed to get user info from GitHub");
                throw new InvalidOperationException("Failed to retrieve user information from GitHub");
            }

            _logger.LogInformation("Successfully retrieved GitHub user info for: {Username}", userInfo.Login);

            return new GitHubOAuthResponse
            {
                AccessToken = accessToken,
                TokenType = "bearer",
                Scope = _configuration["GitHub:Scopes"] ?? "repo,read:user",
                GitHubUsername = userInfo.Login,
                GitHubUserId = userInfo.Id,
                Email = userInfo.Email,
                ConnectedAt = DateTime.UtcNow
            };            
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during GitHub user info retrieval");
            throw;
        }
    }

    // Internal DTOs for GitHub API responses    
    private class GitHubTokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }

    private class GitHubUserInfo
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public long Id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string? Email { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
    }

}