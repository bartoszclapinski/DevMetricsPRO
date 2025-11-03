using DevMetricsPro.Application.DTOs.GitHub;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DevMetricsPro.Core.Enums;

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
    private readonly IGitHubRepositoryService _githubRepositoryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGitHubCommitsService _gitHubCommitsService;

    public GitHubController(IGitHubOAuthService gitHubOAuthService,
        UserManager<ApplicationUser> userManager,
        ILogger<GitHubController> logger,
        IGitHubRepositoryService githubRepositoryService,
        IUnitOfWork unitOfWork,
        IGitHubCommitsService gitHubCommitsService)
    {
        _gitHubOAuthService = gitHubOAuthService ?? throw new ArgumentNullException(nameof(gitHubOAuthService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _githubRepositoryService = githubRepositoryService ?? throw new ArgumentNullException(nameof(githubRepositoryService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _gitHubCommitsService = gitHubCommitsService ?? throw new ArgumentNullException(nameof(gitHubCommitsService));
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

    /// <summary>
    /// Sync repositories from GitHub for the authenticated user
    /// </summary>
    /// <returns>List of synced repositories</returns>
    [HttpPost("sync-repositories")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncRepositories(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting repository sync for user");

            // Get currently logged-in user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Sync repositories called without authenticated user");
                return Unauthorized(new { error = "User not authenticated" });
            }

            // Get user from database with GitHub token
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User {UserId} not found in database", userId);
                return NotFound(new { error = "User not found" });
            }

            // Check if GitHub is connected
            if (string.IsNullOrEmpty(user.GitHubAccessToken))
            {
                _logger.LogWarning("User {UserId} attempted to sync without connected GitHub account", userId);
                return BadRequest(new {error = "GitHub account not connected. Please connect your GitHub account first."});
            }

            // Fetch repositories from GitHub
            _logger.LogInformation("Fetching repositories from GitHub for user {UserId}", userId);
            var githubRepos = await _githubRepositoryService.GetUserRepositoriesAsync(
                user.GitHubAccessToken, cancellationToken);

            // Save repositories to database
            var (addedCount, updatedCount) = await SaveRepositoriesToDatabaseAsync(
                githubRepos, user, cancellationToken);
            
            _logger.LogInformation("Successfully synced {Count} repositories for user {UserId}",
                githubRepos.Count(), userId);

            return Ok(new
            {
                success = true,
                count = githubRepos.Count(),
                repositories = githubRepos
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "GitHub authorization failed during repository sync");
            return Unauthorized(new {error = ex.Message});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing repositories from GitHub");
            return StatusCode(500, new { error = "Failed to sync repositories", detail = ex.Message });
        }
    }

    /// <summary>
    /// Sync commits for a specific repository
    /// </summary>
    [HttpPost("commits/sync/{repositoryId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncCommits(Guid repositoryId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting commit sync for repository {RepositoryId}", repositoryId);

            // Get currently logged-in user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new {error = "User not authenticated"});
            }

            // Get user from database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new {error = "User not found"});
            }

            // Check if GitHub is connected
            if (string.IsNullOrEmpty(user.GitHubAccessToken) || string.IsNullOrEmpty(user.GitHubUsername))
            {
                return BadRequest(new {error = "GitHub account not connected. Please connect your GitHub account first."});
            }

            // Get repository from database
            var repositoryRepo = _unitOfWork.Repository<Repository>();
            var repositories = await repositoryRepo.FindAsync(r => r.Id == repositoryId, cancellationToken);
            var repository = repositories.FirstOrDefault();

            if (repository == null)
            {
                return NotFound(new {error = "Repository not found"});
            }

            // Extract repository owner and repository name
            var owner = user.GitHubUsername;
            var repoName = repository.Name;

            _logger.LogInformation("Fetching commits from GitHub repository {Owner}/{Repo}", owner, repoName);
            
            // Fetch commits from GitHub (only last 100 for MVP)
            var githubCommits = await _gitHubCommitsService.GetRepositoryCommitsAsync(
                owner,
                repoName,
                user.GitHubAccessToken,
                since: repository.LastSyncedAt, // Incremental sync
                cancellationToken);

            // Save commits to database
            var (addedCount, updatedCount) = await SaveCommitsToDatabaseAsync(
                githubCommits, repository, cancellationToken);
            
            // Update repository last synced date
            repository.LastSyncedAt = DateTime.UtcNow;
            await repositoryRepo.UpdateAsync(repository, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                 "Successfully synced {Total} commits for repository {RepositoryId} ({Added} new, {Updated} updated)",
                 addedCount + updatedCount, repositoryId, addedCount, updatedCount);

            return Ok(new 
            {
                success = true,
                repositoryId = repositoryId,
                repositoryName = repoName,
                addedCount = addedCount,
                updatedCount = updatedCount,
                totalCommits = addedCount + updatedCount
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "GitHub authorization failed during commit sync");
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing commits for repository {RepositoryId}", repositoryId);
            return StatusCode(500, new { error = "Failed to sync commits", detail = ex.Message });
        }   
        
    }

    /// <summary>
    /// Get recent commits across all repositories for the authenticated user
    /// </summary>
    [HttpGet("commits/recent")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentCommits(
        [FromQuery] int limit = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get currently logged-in user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new {error = "User not authenticated"});
            }
            
            // Fetch recent commits from database
            var commitRepo = _unitOfWork.Repository<Commit>();
            var commits = await commitRepo.GetAllAsync(cancellationToken);

            // Get most recent commits ordered byt date
            var recentCommits = commits
                .OrderByDescending(c => c.CommittedAt)
                .Take(limit)
                .Select(c => new
                {
                    sha = c.Sha,
                    message = c.Message,
                    authorName = c.Developer?.DisplayName ?? "Unknown Author Name",
                    committedAt = c.CommittedAt,
                    repositoryName = c.Repository?.Name ?? "Unknown Repository Name",
                    linesAdded = c.LinesAdded,
                    linesRemoved = c.LinesRemoved
                })
                .ToList();
            
            // Get total commit count
            var totalCommits = commits.Count();

            return Ok(new
            {
                success = true,
                totalCommits = totalCommits,
                recentCommits = recentCommits
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent commits");
            return StatusCode(500, new { error = "Failed to fetch recent commits", detail = ex.Message });
        }
    }
   

    /// <summary>
    /// Saves or updates repositories in the database
    /// </summary>
    /// <param name="githubRepos">GitHub repository DTOs to save</param>
    /// <param name="user">Current application user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple with (addedCount, updatedCount)</returns>
    private async Task<(int addedCount, int updatedCount)> SaveRepositoriesToDatabaseAsync(
        IEnumerable<GitHubRepositoryDto> githubRepos,
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var repositoryRepo = _unitOfWork.Repository<Repository>();
        int addedCount = 0;
        int updatedCount = 0;

        foreach (var githubRepo in githubRepos)
        {
            // Check if repository already exists by ExternalId (GitHub ID)
            var existingRepos = await repositoryRepo.FindAsync(
                r => r.ExternalId == githubRepo.Id.ToString() && r.Platform == PlatformType.GitHub,
                cancellationToken);
            
            var existingRepo = existingRepos.FirstOrDefault();

            if (existingRepo != null)
            {
                // Update existing repository
                existingRepo.Name = githubRepo.Name;
                existingRepo.Description = githubRepo.Description;
                existingRepo.Url = githubRepo.HtmlUrl;
                existingRepo.FullName = $"{user.GitHubUsername}/{githubRepo.Name}";
                existingRepo.IsPrivate = githubRepo.IsPrivate;
                existingRepo.IsFork = githubRepo.IsFork;
                existingRepo.StargazersCount = githubRepo.StargazersCount;
                existingRepo.ForksCount = githubRepo.ForksCount;
                existingRepo.OpenIssuesCount = githubRepo.OpenIssuesCount;
                existingRepo.Language = githubRepo.Language;
                existingRepo.PushedAt = githubRepo.PushedAt;
                existingRepo.LastSyncedAt = DateTime.UtcNow;
                existingRepo.UpdatedAt = DateTime.UtcNow;

                await repositoryRepo.UpdateAsync(existingRepo, cancellationToken);
                updatedCount++;
                _logger.LogDebug("Updated existing repository {RepoName}", githubRepo.Name);
            }
            else
            {
                // Create new repository
                var newRepo = new Repository
                {
                    Id = Guid.NewGuid(),
                    Name = githubRepo.Name,
                    Description = githubRepo.Description,
                    Platform = PlatformType.GitHub,
                    ExternalId = githubRepo.Id.ToString(),
                    Url = githubRepo.HtmlUrl,
                    DefaultBranch = "main", // GitHub default
                    FullName = $"{user.GitHubUsername}/{githubRepo.Name}",
                    IsPrivate = githubRepo.IsPrivate,
                    IsFork = githubRepo.IsFork,
                    StargazersCount = githubRepo.StargazersCount,
                    ForksCount = githubRepo.ForksCount,
                    OpenIssuesCount = githubRepo.OpenIssuesCount,
                    Language = githubRepo.Language,
                    PushedAt = githubRepo.PushedAt,
                    IsActive = true,
                    LastSyncedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await repositoryRepo.AddAsync(newRepo, cancellationToken);
                addedCount++;
                _logger.LogDebug("Added new repository {RepoName}", githubRepo.Name);
            }
        }

        // Save all changes to database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Repository sync complete: {Added} added, {Updated} updated",
            addedCount, updatedCount);

        return (addedCount, updatedCount);
    }

    /// <summary>
    /// Saves commits to database with upsert logic
    /// </summary>
    private async Task<(int addedCount, int updatedCount)> SaveCommitsToDatabaseAsync(
        IEnumerable<GitHubCommitDto> githubCommits,
        Repository repository,
        CancellationToken cancellationToken)
    {
        var commitRepo = _unitOfWork.Repository<Commit>();
        var developerRepo = _unitOfWork.Repository<Developer>();
        int addedCount = 0;
        int updatedCount = 0;

        foreach (var githubCommit in githubCommits)
        {
            // Check if commit already exists by SHA
            var existingCommits = await commitRepo.FindAsync(
                c => c.Sha == githubCommit.Sha && c.RepositoryId == repository.Id,
                cancellationToken);
            var existingCommit = existingCommits.FirstOrDefault();

            // Find or create developer by email
            var developers = await developerRepo.FindAsync(
                d => d.Email == githubCommit.AuthorEmail,
                cancellationToken);
            var developer = developers.FirstOrDefault();

            if (developer == null)
            {
                // Create new developer
                developer = new Developer
                {
                    Id = Guid.NewGuid(),
                    DisplayName = githubCommit.AuthorName,
                    Email = githubCommit.AuthorEmail,
                    GitHubUsername = githubCommit.AuthorEmail.Split('@')[0], // Temporary
                    CreatedAt = DateTime.UtcNow
                };
                
                await developerRepo.AddAsync(developer, cancellationToken);
                _logger.LogDebug("Created new developer {Email}", developer.Email);
            }
            
            if (existingCommit != null)
            {
                // Update existing commit
                existingCommit.Message = githubCommit.Message;
                existingCommit.LinesAdded = githubCommit.Additions;
                existingCommit.LinesRemoved = githubCommit.Deletions;
                existingCommit.FilesChanged = githubCommit.TotalChanges > 0 ? githubCommit.TotalChanges : 1;
                existingCommit.CommittedAt = githubCommit.CommitterDate;
                existingCommit.DeveloperId = developer.Id;
                existingCommit.UpdatedAt = DateTime.UtcNow;

                await commitRepo.UpdateAsync(existingCommit, cancellationToken);
                updatedCount++;
                _logger.LogDebug("Updated existing commit {Sha}", githubCommit.Sha);
            }      
            else
            {
                // Create new commit
                var newCommit = new Commit
                {
                    Id = Guid.NewGuid(),
                    RepositoryId = repository.Id,
                    DeveloperId = developer.Id,
                    Sha = githubCommit.Sha,
                    Message = githubCommit.Message,
                    LinesAdded = githubCommit.Additions,
                    LinesRemoved = githubCommit.Deletions,
                    FilesChanged = githubCommit.TotalChanges > 0 ? githubCommit.TotalChanges : 1,
                    CommittedAt = githubCommit.CommitterDate,
                    CreatedAt = DateTime.UtcNow
                };

                await commitRepo.AddAsync(newCommit, cancellationToken);
                addedCount++;
                _logger.LogDebug("Added new commit {Sha}", githubCommit.Sha);
            }            
        }

        // Save all changes to database
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Commit sync complete: {Added} added, {Updated} updated",
                addedCount, updatedCount);

            return (addedCount, updatedCount);
    }
}
