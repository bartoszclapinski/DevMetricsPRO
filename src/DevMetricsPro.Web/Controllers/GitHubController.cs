using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevMetricsPro.Web.Controllers;

/// <summary>
/// API controller for GitHub OAuth integration
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class GitHubController : ControllerBase
{
    private readonly IGitHubOAuthService _gitHubOAuthService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(IGitHubOAuthService gitHubOAuthService,
        UserManager<ApplicationUser> userManager,
        ILogger<GitHubController> logger)
    {
        _gitHubOAuthService = gitHubOAuthService ?? throw new ArgumentNullException(nameof(gitHubOAuthService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Initiates GitHub OAuth flow
    /// </summary>
    /// <returns>GitHub authorization URL to redirect to</returns>
    [HttpGet("authorize")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetAuthorizationUrl()
    {
        // Get current user ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        // Include user ID in state for callback validation
        var state = $"{Guid.NewGuid()}:{userId}";
        
        // Store state in session for CSRF validation
        HttpContext.Session.SetString("GitHubOAuthState", state);

        var authUrl = _gitHubOAuthService.GetAuthorizationUrl(state);

        _logger.LogInformation("Generated GitHub OAuth URL for user {UserId}", userId);

        return Ok(new { authorizationUrl = authUrl }); 
    }

    /// <summary>
    /// Get GitHub connection status for current user
    /// </summary>
    /// <returns>Connection status with username if connected</returns>
    [HttpGet("status")]
    [Authorize(AuthenticationSchemes = "Bearer")]  // Use JWT Bearer auth
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConnectionStatus()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        var isConnected = !string.IsNullOrEmpty(user.GitHubUsername);

        return Ok(new 
        { 
            connected = isConnected, 
            username = user.GitHubUsername, 
            connectedAt = user.GitHubConnectedAt
        });
    }

    /// <summary>
    /// GitHub OAuth callback endpoint - receives authorization code
    /// </summary>
    /// <param name="code">Authorization code from GitHub</param>
    /// <param name="state">State parameter for CSRF validation and user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or error message</returns>
    [HttpGet("callback")]
    [AllowAnonymous] // GitHub redirects here, so user might not have active session
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Callback(
        [FromQuery] string code,
        [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code))
        {
            _logger.LogWarning("GitHub callback received with missing parameters");
            return Redirect("/?error=invalid-request");
        }

        try
        {
            // Extract user ID from state parameter (format: "guid:userId")
            var stateParts = state.Split(':', 2);
            if (stateParts.Length != 2)
            {
                _logger.LogWarning("Invalid state parameter format");
                return Redirect("/?error=invalid-state");
            }

            var userId = stateParts[1];
            _logger.LogInformation("Processing GitHub callback for user {UserId}", userId);

            // Exchange code for access token and get user info
            var oauthResponse = await _gitHubOAuthService.ExchangeCodeForTokenAsync(
                code, cancellationToken);

            // Find user in database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User not found for ID: {UserId}", userId);
                return Redirect("/?error=user-not-found");
            }

            // Save GitHub information to database
            user.GitHubAccessToken = oauthResponse.AccessToken;
            user.GitHubUsername = oauthResponse.GitHubUsername;
            user.GitHubUserId = oauthResponse.GitHubUserId;
            user.GitHubConnectedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user with GitHub info: {Errors}", 
                    string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                return Redirect("/?error=update-failed");
            }

            _logger.LogInformation(
                "Successfully connected GitHub account {Username} for user {UserId}",
                oauthResponse.GitHubUsername,
                userId
            );

            // Clear the state from session
            HttpContext.Session.Remove("GitHubOAuthState");

            // Redirect to home page with success message
            return Redirect("/?github=connected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GitHub OAuth callback");
            return Redirect("/?error=connection-failed");
        }
    }

    /// <summary>
    /// Test endpoint to check if GitHub integration is working
    /// </summary>
    [HttpGet("test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        return Ok(new { message = "GitHub OAuth controller is working" });
    }
        
}