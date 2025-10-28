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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetAuthorizationUrl()
    {
        // Generate a random state for CSRF protection
        var state = Guid.NewGuid().ToString();

        // Store state in session or temp data for validation on callback
        HttpContext.Session.SetString("GitHubOAuthState", state);

        var authUrl = _gitHubOAuthService.GetAuthorizationUrl(state);

        _logger.LogInformation("Generated GitHub OAuth URL for user");

        return Ok(new { authorizationUrl = authUrl }); 
    }

    /// <summary>
    /// GitHub OAuth callback endpoint - receives authorization code
    /// </summary>
    /// <param name="code">Authorization code from GitHub</param>
    /// <param name="state">State parameter for CSRF validation</param>
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
        // Validate state parameter (CSRF protection)
        // TODO: Fix session state sharing between Blazor and API controllers
        // var savedState = HttpContext.Session.GetString("GitHubOAuthState");
        // if (string.IsNullOrEmpty(savedState) || savedState != state)
        // {
        //     _logger.LogWarning("GitHub OAuth state mismatch - possible CSRF attack");
        //     return BadRequest(new { error = "Invalid state parameter"});
        // }

        try
        {
            // Exchange code for access token and get user info
            var oauthResponse = await _gitHubOAuthService.ExchangeCodeForTokenAsync(
                code, cancellationToken);
            
            // TODO: Fix user authentication context preservation during OAuth redirect
            // For now, temporarily disabled for testing OAuth token exchange
            /*
            // Get currently logged-in user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GitHub callback received but user not authenticated");
                return Unauthorized(new { error = "User must be logged in"});
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { error = "User not found"});
            }
            */

            _logger.LogInformation("User authentication check temporarily disabled for OAuth testing");

            // TODO: Store access token and GitHub info in database
            // For now just log success
            _logger.LogInformation(
                "Successfully authenticated GitHub user {Username} with email {Email}",
                oauthResponse.GitHubUsername,
                oauthResponse.Email
            );

            // Clear the state from session
            HttpContext.Session.Remove("GitHubOAuthState");

            // Redirect to home page (TODO: Create /settings page)
            return Redirect("/?github=connected");       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GitHub OAuth callback");
            return BadRequest(new { error = "Failed to connect GitHub account", detail = ex.Message });
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