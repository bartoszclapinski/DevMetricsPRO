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
    private readonly IGitHubPullRequestService _gitHubPullRequestService;

    public GitHubController(IGitHubOAuthService gitHubOAuthService,
        UserManager<ApplicationUser> userManager,
        ILogger<GitHubController> logger,
        IGitHubRepositoryService githubRepositoryService,
        IUnitOfWork unitOfWork,
        IGitHubCommitsService gitHubCommitsService,
        IGitHubPullRequestService gitHubPullRequestService)
    {
        _gitHubOAuthService = gitHubOAuthService ?? throw new ArgumentNullException(nameof(gitHubOAuthService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _githubRepositoryService = githubRepositoryService ?? throw new ArgumentNullException(nameof(githubRepositoryService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _gitHubCommitsService = gitHubCommitsService ?? throw new ArgumentNullException(nameof(gitHubCommitsService));
        _gitHubPullRequestService = gitHubPullRequestService ?? throw new ArgumentNullException(nameof(gitHubPullRequestService));
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
    [HttpPost("commits/sync/{githubRepositoryId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncCommits(long githubRepositoryId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting commit sync for GitHub repository {GitHubRepositoryId}", githubRepositoryId);

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

            // Get repository from database by GitHub ID (stored as ExternalId)
            var repositoryRepo = _unitOfWork.Repository<Repository>();
            var githubIdString = githubRepositoryId.ToString();
            var repositories = await repositoryRepo.FindAsync(r => r.ExternalId == githubIdString, cancellationToken);
            var repository = repositories.FirstOrDefault();

            if (repository == null)
            {
                return NotFound(new {error = $"Repository with GitHub ID {githubRepositoryId} not found in database. Please sync repositories first."});
            }
            
            var repositoryId = repository.Id; // Internal Guid for logging

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
            _logger.LogError(ex, "Error syncing commits for repository {GitHubRepositoryId}", githubRepositoryId);
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
                // Don't set LastSyncedAt here - only set it when commits are synced
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
                    // Don't set LastSyncedAt here - only set it when commits are synced
                    LastSyncedAt = null,
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

        // Track developers we've already processed in this batch to avoid duplicates
        var processedDevelopers = new Dictionary<string, Developer>();

        foreach (var githubCommit in githubCommits)
        {
            // Check if commit already exists by SHA
            var existingCommits = await commitRepo.FindAsync(
                c => c.Sha == githubCommit.Sha && c.RepositoryId == repository.Id,
                cancellationToken);
            var existingCommit = existingCommits.FirstOrDefault();

            // Find or create developer by email
            Developer? developer;
            
            // First check if we've already processed this developer in this batch
            if (processedDevelopers.ContainsKey(githubCommit.AuthorEmail))
            {
                developer = processedDevelopers[githubCommit.AuthorEmail];
            }
            else
            {
                // Check database for existing developer
                var developers = await developerRepo.FindAsync(
                    d => d.Email == githubCommit.AuthorEmail,
                    cancellationToken);
                developer = developers.FirstOrDefault();

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
                
                // Add to processed cache
                processedDevelopers[githubCommit.AuthorEmail] = developer;
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

    /// <summary>
    /// Syncs pull requests from GitHub for a specific repository
    /// </summary>
    /// <param name="repositoryId">The repository ID to sync PRs for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync statistics</returns>
    [HttpPost("pull-requests/sync/{repositoryId:guid}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SyncPullRequests(
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting PR sync for repository {RepositoryId}", repositoryId);

            // Get authenticated user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in JWT claims");
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Get user with GitHub connection
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return BadRequest(new { message = "User not found" });
            }

            if (string.IsNullOrEmpty(user.GitHubAccessToken))
            {
                _logger.LogWarning("User {UserId} does not have GitHub connected", userId);
                return BadRequest(new { message = "GitHub account not connected" });
            }

            // Get repository
            var repoRepository = _unitOfWork.Repository<Core.Entities.Repository>();
            var repositories = await repoRepository.GetAllAsync(cancellationToken);
            var repository = repositories.FirstOrDefault(r => r.Id == repositoryId);

            if (repository == null)
            {
                _logger.LogWarning("Repository {RepositoryId} not found", repositoryId);
                return NotFound(new { message = "Repository not found" });
            }

            // Parse owner/repo from FullName
            var parts = repository.FullName?.Split('/');
            if (parts == null || parts.Length != 2)
            {
                _logger.LogWarning("Invalid repository FullName format: {FullName}", repository.FullName);
                return BadRequest(new { message = "Invalid repository format" });
            }

            var owner = parts[0];
            var repoName = parts[1];

            // Fetch PRs from GitHub (incremental sync using LastSyncedAt)
            var pullRequests = await _gitHubPullRequestService.GetRepositoryPullRequestsAsync(
                owner,
                repoName,
                user.GitHubAccessToken,
                repository.LastSyncedAt,
                cancellationToken);

            var prList = pullRequests.ToList();
            _logger.LogInformation("Fetched {Count} pull requests from GitHub", prList.Count);

            // Save PRs to database
            var (addedCount, updatedCount) = await SavePullRequestsToDatabaseAsync(
                prList,
                repository.Id,
                cancellationToken);

            // Update LastSyncedAt
            repository.LastSyncedAt = DateTime.UtcNow;
            await repoRepository.UpdateAsync(repository, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "PR sync complete for repository {RepositoryId}: {Added} added, {Updated} updated",
                repositoryId, addedCount, updatedCount);

            return Ok(new
            {
                success = true,
                message = "Pull requests synced successfully",
                added = addedCount,
                updated = updatedCount,
                total = addedCount + updatedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing pull requests for repository {RepositoryId}", repositoryId);
            return StatusCode(500, new { message = "Failed to sync pull requests" });
        }
    }

    /// <summary>
    /// Saves pull requests to database with upsert logic
    /// </summary>
    private async Task<(int addedCount, int updatedCount)> SavePullRequestsToDatabaseAsync(
        List<GitHubPullRequestDto> githubPRs,
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {
        var addedCount = 0;
        var updatedCount = 0;
        var prRepository = _unitOfWork.Repository<PullRequest>();
        var developerRepository = _unitOfWork.Repository<Developer>();

        // Cache processed developers to avoid duplicate queries
        var processedDevelopers = new Dictionary<string, Developer>();

        foreach (var githubPR in githubPRs)
        {
            // Check if PR already exists (by ExternalId + repository)
            var existingPRs = await prRepository.GetAllAsync(cancellationToken);
            var existingPR = existingPRs.FirstOrDefault(pr =>
                pr.ExternalId == githubPR.Number.ToString() &&
                pr.RepositoryId == repositoryId);

            // Find or create developer (author)
            Developer developer;
            if (processedDevelopers.ContainsKey(githubPR.AuthorLogin))
            {
                developer = processedDevelopers[githubPR.AuthorLogin];
            }
            else
            {
                var developers = await developerRepository.GetAllAsync(cancellationToken);
                developer = developers.FirstOrDefault(d =>
                    d.GitHubUsername == githubPR.AuthorLogin)!;

                if (developer == null)
                {
                    // Create new developer
                    developer = new Developer
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = githubPR.AuthorName ?? githubPR.AuthorLogin,
                        Email = $"{githubPR.AuthorLogin}@github.user", // Placeholder email
                        GitHubUsername = githubPR.AuthorLogin,
                        CreatedAt = DateTime.UtcNow
                    };

                    await developerRepository.AddAsync(developer, cancellationToken);
                    _logger.LogDebug("Created new developer {GitHubUsername}", developer.GitHubUsername);
                }

                processedDevelopers[githubPR.AuthorLogin] = developer;
            }

            // Map PR status
            var prStatus = githubPR.State.ToLower() switch
            {
                "open" => PullRequestStatus.Open,
                "closed" when githubPR.IsMerged => PullRequestStatus.Merged,
                "closed" => PullRequestStatus.Closed,
                _ => PullRequestStatus.Open
            };

            if (existingPR != null)
            {
                // Update existing PR
                existingPR.Title = githubPR.Title;
                existingPR.Description = githubPR.Body;
                existingPR.Status = prStatus;
                existingPR.AuthorId = developer.Id;
                existingPR.CreatedAt = githubPR.CreatedAt;
                existingPR.ClosedAt = githubPR.ClosedAt;
                existingPR.MergedAt = githubPR.MergedAt;
                existingPR.UpdatedAt = DateTime.UtcNow;

                await prRepository.UpdateAsync(existingPR, cancellationToken);
                updatedCount++;
                _logger.LogDebug("Updated existing PR #{Number}", githubPR.Number);
            }
            else
            {
                // Create new PR
                var newPR = new PullRequest
                {
                    Id = Guid.NewGuid(),
                    RepositoryId = repositoryId,
                    AuthorId = developer.Id,
                    ExternalId = githubPR.Number.ToString(),
                    Title = githubPR.Title,
                    Description = githubPR.Body,
                    Status = prStatus,
                    CreatedAt = githubPR.CreatedAt,
                    ClosedAt = githubPR.ClosedAt,
                    MergedAt = githubPR.MergedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                await prRepository.AddAsync(newPR, cancellationToken);
                addedCount++;
                _logger.LogDebug("Added new PR #{Number}", githubPR.Number);
            }
        }

        // Save all changes to database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "PR sync complete: {Added} added, {Updated} updated",
            addedCount, updatedCount);

        return (addedCount, updatedCount);
    }

    /// <summary>
    /// Triggers a background job to sync all GitHub data (repositories and commits) for the current user
    /// </summary>
    /// <returns>Job ID</returns>
    [HttpPost("sync-all")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TriggerFullSync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get authenticated user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in JWT claims");
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return BadRequest(new { message = "User not found" });
            }

            if (string.IsNullOrEmpty(user.GitHubAccessToken))
            {
                _logger.LogWarning("User {UserId} does not have GitHub connected", userId);
                return BadRequest(new { message = "GitHub account not connected" });
            }

            // Enqueue background job
            var jobId = Hangfire.BackgroundJob.Enqueue<Web.Jobs.SyncGitHubDataJob>(
                job => job.ExecuteAsync(user.Id));

            _logger.LogInformation(
                "Enqueued full GitHub sync job {JobId} for user {UserId}",
                jobId, userId);

            return Ok(new
            {
                success = true,
                message = "GitHub sync job started",
                jobId = jobId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering full GitHub sync");
            return StatusCode(500, new { message = "Failed to start sync job" });
        }
    }

    /// <summary>
    /// Get all pull requests from database with optional filtering
    /// </summary>
    [HttpGet("pull-requests")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPullRequests(
        [FromQuery] Guid? repositoryId = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get authenticated user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            // Fetch pull requests from database
            var prRepository = _unitOfWork.Repository<PullRequest>();
            var pullRequests = await prRepository.GetAllAsync(cancellationToken);

            // Filter by repository if provided
            if (repositoryId.HasValue)
            {
                pullRequests = pullRequests.Where(pr => pr.RepositoryId == repositoryId.Value);
            }

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                var prStatus = status.ToLower() switch
                {
                    "open" => PullRequestStatus.Open,
                    "closed" => PullRequestStatus.Closed,
                    "merged" => PullRequestStatus.Merged,                    
                    _ => (PullRequestStatus?)null
                };

                if (prStatus.HasValue)
                {
                    pullRequests = pullRequests.Where(pr => pr.Status == prStatus.Value);
                }
            }

            // Order by most recent first
            var sortedPRs = pullRequests
                .OrderByDescending(pr => pr.UpdatedAt)
                .Select(pr => new
                {
                    id = pr.Id,
                    externalId = pr.ExternalId,
                    title = pr.Title,
                    description = pr.Description,
                    status = pr.Status.ToString(),
                    isMerged = pr.Status == PullRequestStatus.Merged,
                    authorName = pr.Author?.DisplayName ?? "Unknown",
                    authorUsername = pr.Author?.GitHubUsername ?? "Unknown",
                    repositoryName = pr.Repository?.Name ?? "Unknown",
                    repositoryId = pr.RepositoryId,
                    createdAt = pr.CreatedAt,
                    updatedAt = pr.UpdatedAt,
                    closedAt = pr.ClosedAt,
                    mergedAt = pr.MergedAt,
                    // GitHub URL format https://github.com/{owner}/{repository}/pull/{externalId}
                    url = pr.Repository?.FullName != null ? 
                    $"https://github.com/{pr.Repository?.FullName}/pull/{pr.ExternalId}" : null
                })
                .ToList();

                _logger.LogInformation("Fetched {Count} pull requests from database", sortedPRs.Count);

                return Ok(new
                {
                    success = true,
                    count = sortedPRs.Count,
                    pullRequests = sortedPRs
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pull requests from database");
            return StatusCode(500, new { message = "Failed to fetch pull requests from database" , detail = ex.Message });
        }
    }
    
}
