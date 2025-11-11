using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Enums;
using DevMetricsPro.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DevMetricsPro.Web.Jobs;

/// <summary>
/// Background job for syncing GitHub data (repositories, commits, and pull requests) for a user.
/// This job is executed by Hangfire on a schedule or manually triggered.
/// </summary>
public class SyncGitHubDataJob
{
    private readonly IGitHubRepositoryService _repositoryService;
    private readonly IGitHubCommitsService _commitsService;
    private readonly IGitHubPullRequestService _pullRequestService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SyncGitHubDataJob> _logger;

    public SyncGitHubDataJob(
        IGitHubRepositoryService repositoryService,
        IGitHubCommitsService commitsService,
        IGitHubPullRequestService pullRequestService,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogger<SyncGitHubDataJob> logger)
    {
        _repositoryService = repositoryService;
        _commitsService = commitsService;
        _pullRequestService = pullRequestService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Executes the GitHub data sync for a specific user.
    /// This method syncs repositories first, then commits, then pull requests for each repository.
    /// </summary>
    /// <param name="userId">The ID of the user to sync data for</param>
    public async Task ExecuteAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Starting GitHub data sync for user {UserId}", userId);

            // Get user with GitHub connection
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return;
            }

            if (string.IsNullOrEmpty(user.GitHubAccessToken))
            {
                _logger.LogWarning("User {UserId} does not have GitHub connected", userId);
                return;
            }

            // Step 1: Sync repositories
            _logger.LogInformation("Syncing repositories for user {UserId}", userId);
            var repositories = await _repositoryService.GetUserRepositoriesAsync(
                user.GitHubAccessToken, 
                CancellationToken.None);

            var repositoryList = repositories.ToList();
            _logger.LogInformation("Found {Count} repositories for user {UserId}", 
                repositoryList.Count, userId);

            // Save repositories to database
            var syncedRepos = await SaveRepositoriesToDatabaseAsync(repositoryList);
            _logger.LogInformation("Saved {Count} repositories to database", syncedRepos.Count);

            // Step 2: Sync commits for each repository
            var totalCommitsSynced = 0;
            foreach (var repo in syncedRepos)
            {
                try
                {
                    _logger.LogInformation("Syncing commits for repository {RepoName}", repo.Name);

                    // Parse owner/repo from FullName
                    var parts = repo.FullName?.Split('/');
                    if (parts == null || parts.Length != 2)
                    {
                        _logger.LogWarning("Invalid repository FullName format: {FullName}", repo.FullName);
                        continue;
                    }

                    var owner = parts[0];
                    var repoName = parts[1];

                    // Fetch commits (incremental sync using LastSyncedAt)
                    var commits = await _commitsService.GetRepositoryCommitsAsync(
                        owner,
                        repoName,
                        user.GitHubAccessToken,
                        repo.LastSyncedAt,
                        CancellationToken.None);

                    var commitsList = commits.ToList();
                    _logger.LogInformation("Found {Count} commits for repository {RepoName}", 
                        commitsList.Count, repoName);

                    // Save commits to database
                    if (commitsList.Any())
                    {
                        var (added, updated) = await SaveCommitsToDatabaseAsync(commitsList, repo.Id);
                        totalCommitsSynced += added + updated;

                        // Update LastSyncedAt
                        repo.LastSyncedAt = DateTime.UtcNow;
                        await _unitOfWork.Repository<Core.Entities.Repository>().UpdateAsync(repo);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing commits for repository {RepoName}", repo.Name);
                    // Continue with next repository
                }
            }

            // Step 3: Sync pull requests for each repository
            var totalPRsSynced = 0;
            foreach (var repo in syncedRepos)
            {
                try
                {
                    _logger.LogInformation("Syncing pull requests for repository {RepoName}", repo.Name);

                    // Parse owner/repo from FullName
                    var parts = repo.FullName?.Split('/');
                    if (parts == null || parts.Length != 2)
                    {
                        _logger.LogWarning("Invalid repository FullName format: {FullName}", repo.FullName);
                        continue;
                    }

                    var owner = parts[0];
                    var repoName = parts[1];

                    // Fetch PRs (incremental sync using LastSyncedAt)
                    var pullRequests = await _pullRequestService.GetRepositoryPullRequestsAsync(
                        owner,
                        repoName,
                        user.GitHubAccessToken,
                        repo.LastSyncedAt,
                        CancellationToken.None);

                    var prList = pullRequests.ToList();
                    _logger.LogInformation("Found {Count} pull requests for repository {RepoName}", 
                        prList.Count, repoName);

                    // Save PRs to database
                    if (prList.Any())
                    {
                        var (added, updated) = await SavePullRequestsToDatabaseAsync(prList, repo.Id);
                        totalPRsSynced += added + updated;

                        _logger.LogInformation(
                            "Synced {Count} pull requests for repository {RepoName} ({Added} added, {Updated} updated)",
                            added + updated, repoName, added, updated);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing pull requests for repository {RepoName}", repo.Name);
                    // Continue with next repository
                }
            }

            _logger.LogInformation(
                "GitHub data sync completed for user {UserId}. " +
                "Synced {RepoCount} repositories, {CommitCount} commits, and {PRCount} pull requests",
                userId, syncedRepos.Count, totalCommitsSynced, totalPRsSynced);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GitHub data sync for user {UserId}", userId);
            throw; // Re-throw to let Hangfire handle retry
        }
    }

    private async Task<List<Core.Entities.Repository>> SaveRepositoriesToDatabaseAsync(
        List<Application.DTOs.GitHub.GitHubRepositoryDto> githubRepos)
    {
        var savedRepos = new List<Core.Entities.Repository>();
        var repoRepository = _unitOfWork.Repository<Core.Entities.Repository>();

        foreach (var githubRepo in githubRepos)
        {
            // Check if repository already exists
            var existingRepos = await repoRepository.GetAllAsync();
            var existingRepo = existingRepos.FirstOrDefault(r =>
                r.ExternalId == githubRepo.Id.ToString() &&
                r.Platform == PlatformType.GitHub);

            if (existingRepo != null)
            {
                // Update existing repository
                existingRepo.Name = githubRepo.Name;
                existingRepo.Description = githubRepo.Description;
                existingRepo.Url = githubRepo.HtmlUrl;
                existingRepo.FullName = githubRepo.FullName;
                existingRepo.IsPrivate = githubRepo.IsPrivate;
                existingRepo.IsFork = githubRepo.IsFork;
                existingRepo.StargazersCount = githubRepo.StargazersCount;
                existingRepo.ForksCount = githubRepo.ForksCount;
                existingRepo.OpenIssuesCount = githubRepo.OpenIssuesCount;
                existingRepo.Language = githubRepo.Language;
                existingRepo.PushedAt = githubRepo.PushedAt;
                existingRepo.UpdatedAt = DateTime.UtcNow;

                await repoRepository.UpdateAsync(existingRepo);
                savedRepos.Add(existingRepo);
            }
            else
            {
                // Create new repository
                var newRepo = new Core.Entities.Repository
                {
                    Name = githubRepo.Name,
                    Description = githubRepo.Description,
                    Platform = PlatformType.GitHub,
                    ExternalId = githubRepo.Id.ToString(),
                    Url = githubRepo.HtmlUrl,
                    FullName = githubRepo.FullName,
                    IsPrivate = githubRepo.IsPrivate,
                    IsFork = githubRepo.IsFork,
                    StargazersCount = githubRepo.StargazersCount,
                    ForksCount = githubRepo.ForksCount,
                    OpenIssuesCount = githubRepo.OpenIssuesCount,
                    Language = githubRepo.Language,
                    PushedAt = githubRepo.PushedAt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var added = await repoRepository.AddAsync(newRepo);
                savedRepos.Add(added);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return savedRepos;
    }

    private async Task<(int addedCount, int updatedCount)> SaveCommitsToDatabaseAsync(
        List<Application.DTOs.GitHub.GitHubCommitDto> githubCommits,
        Guid repositoryId)
    {
        var addedCount = 0;
        var updatedCount = 0;
        var commitRepository = _unitOfWork.Repository<Commit>();
        var developerRepository = _unitOfWork.Repository<Developer>();

        foreach (var githubCommit in githubCommits)
        {
            // Check if commit already exists
            var existingCommits = await commitRepository.GetAllAsync();
            var existingCommit = existingCommits.FirstOrDefault(c =>
                c.Sha == githubCommit.Sha &&
                c.RepositoryId == repositoryId);

            // Find or create developer
            var developers = await developerRepository.GetAllAsync();
            var developer = developers.FirstOrDefault(d =>
                d.Email == githubCommit.CommitterEmail);

            if (developer == null)
            {
                // Create new developer
                developer = new Developer
                {
                    DisplayName = githubCommit.CommitterName,
                    Email = githubCommit.CommitterEmail,
                    GitHubUsername = githubCommit.CommitterEmail.Split('@')[0], // Temporary
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                developer = await developerRepository.AddAsync(developer);
                await _unitOfWork.SaveChangesAsync(); // Save to get ID
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

                await commitRepository.UpdateAsync(existingCommit);
                updatedCount++;
            }
            else
            {
                // Create new commit
                var newCommit = new Commit
                {
                    Sha = githubCommit.Sha,
                    Message = githubCommit.Message,
                    CommittedAt = githubCommit.CommitterDate,
                    LinesAdded = githubCommit.Additions,
                    LinesRemoved = githubCommit.Deletions,
                    FilesChanged = githubCommit.TotalChanges > 0 ? githubCommit.TotalChanges : 1,
                    RepositoryId = repositoryId,
                    DeveloperId = developer.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await commitRepository.AddAsync(newCommit);
                addedCount++;
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return (addedCount, updatedCount);
    }

    private async Task<(int addedCount, int updatedCount)> SavePullRequestsToDatabaseAsync(
        List<Application.DTOs.GitHub.GitHubPullRequestDto> githubPRs,
        Guid repositoryId)
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
            var existingPRs = await prRepository.GetAllAsync();
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
                var developers = await developerRepository.GetAllAsync();
                developer = developers.FirstOrDefault(d =>
                    d.GitHubUsername == githubPR.AuthorLogin)!;

                if (developer == null)
                {
                    // Create new developer
                    developer = new Developer
                    {
                        DisplayName = githubPR.AuthorName ?? githubPR.AuthorLogin,
                        Email = $"{githubPR.AuthorLogin}@github.user", // Placeholder email
                        GitHubUsername = githubPR.AuthorLogin,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    developer = await developerRepository.AddAsync(developer);
                    await _unitOfWork.SaveChangesAsync(); // Save to get ID
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
                existingPR.ClosedAt = githubPR.ClosedAt;
                existingPR.MergedAt = githubPR.MergedAt;
                existingPR.UpdatedAt = DateTime.UtcNow;

                await prRepository.UpdateAsync(existingPR);
                updatedCount++;
            }
            else
            {
                // Create new PR
                var newPR = new PullRequest
                {
                    RepositoryId = repositoryId,
                    AuthorId = developer.Id,
                    ExternalId = githubPR.Number.ToString(),
                    Title = githubPR.Title,
                    Description = githubPR.Body,
                    Status = prStatus,
                    ClosedAt = githubPR.ClosedAt,
                    MergedAt = githubPR.MergedAt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await prRepository.AddAsync(newPR);
                addedCount++;
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return (addedCount, updatedCount);
    }
}

