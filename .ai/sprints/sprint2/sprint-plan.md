# Sprint 2: GitHub Integration & Data Pipeline 🔄

**Duration**: 2 weeks (10 working days)  
**Goal**: Working GitHub OAuth, data sync, and background processing  
**Commitment**: 20-30 hours total (~2-3 hours/day)  

---

## 📋 Sprint Overview

This sprint integrates with GitHub to fetch and sync developer metrics:
- **Week 1**: GitHub OAuth, API integration, repository sync
- **Week 2**: Background jobs, metrics calculation, data aggregation

Each phase builds on the previous, allowing incremental testing and validation.

---

## 🎯 Sprint Goals

- ✅ GitHub OAuth integration complete (Phase 2.1) ✅
- [ ] GitHub tokens stored securely in database
- [ ] Repositories fetched from GitHub API
- [ ] Commits synced in background
- [ ] Pull requests fetched and stored
- [ ] Basic metrics calculated
- [ ] Hangfire configured for background jobs

---

# WEEK 1: GitHub Integration

## Phase 2.1: GitHub OAuth Setup ✅ (Completed Oct 28, 2025)

**Status**: ✅ Complete  
**Issue**: #44  
**Branch**: `sprint2/phase2.1-github-oauth-#44`  

### What Was Done:
- Created GitHub OAuth DTOs (GitHubOAuthRequest, GitHubOAuthResponse, GitHubCallbackRequest)
- Implemented IGitHubOAuthService interface
- Implemented GitHubOAuthService with:
  - Token exchange (form-encoded POST)
  - User info retrieval from GitHub API
  - JSON property name mapping (snake_case → PascalCase)
- Created GitHubController with authorize and callback endpoints
- Added "Connect GitHub" button to Home.razor
- Configured session support for OAuth state
- Added HttpClient to DI container
- Registered IGitHubOAuthService in DI

### Key Learnings:
- GitHub OAuth requires form-encoded requests, not JSON
- GitHub API responses use snake_case (need [JsonPropertyName] attributes)
- Session state doesn't persist between Blazor Server and API controllers
- OAuth codes can only be used once
- Temporarily disabled state validation and user auth checks for testing

### Testing:
- ✅ Full OAuth flow working end-to-end
- ✅ Authorization URL generated correctly
- ✅ GitHub redirects back with code
- ✅ Token exchange successful
- ✅ User info retrieved from GitHub API
- ✅ Logs show: `Successfully authenticated GitHub user bartoszclapinski`

### Time Spent: ~4 hours

---

## Phase 2.2: Store GitHub Tokens in Database (Day 2-3)

**Goal**: Securely store GitHub access tokens and user info in database

### Step 2.2.1: Add GitHub fields to ApplicationUser

- [ ] **Update `src/DevMetricsPro.Core/Entities/ApplicationUser.cs`**:
  - Add `GitHubAccessToken` (encrypted)
  - Add `GitHubRefreshToken` (if used)
  - Add `GitHubUsername`
  - Add `GitHubUserId`
  - Add `GitHubConnectedAt`

- [ ] **Create EF Core Migration**:
  ```powershell
  dotnet ef migrations add AddGitHubFieldsToUser --project src/DevMetricsPro.Infrastructure --startup-project src/DevMetricsPro.Web
  ```

- [ ] **Apply Migration**:
  ```powershell
  dotnet ef database update --project src/DevMetricsPro.Infrastructure --startup-project src/DevMetricsPro.Web
  ```

**✅ Test**: Check database - new columns should exist in AspNetUsers table

---

### Step 2.2.2: Update GitHubController to Save Tokens

- [ ] **Uncomment and fix user authentication in `GitHubController.Callback`**:
  - Re-enable user ID retrieval
  - Store GitHub token in ApplicationUser
  - Update GitHubUsername, GitHubUserId, GitHubConnectedAt
  - Use UserManager.UpdateAsync() to save

- [ ] **Add encryption for access token** (optional for MVP, recommended):
  - Create ITokenEncryptionService
  - Implement basic encryption using Data Protection API
  - Encrypt before saving, decrypt when retrieving

**✅ Test**: 
- Connect GitHub account
- Check database - user record should have GitHub info
- Verify LastLoginAt updated

---

### Step 2.2.3: Update UI to Show GitHub Connection Status

- [ ] **Update `Home.razor`**:
  - Show "GitHub Connected: @username" if connected
  - Show "Connect GitHub" button if not connected
  - Add "Disconnect GitHub" button

- [ ] **Create API endpoint to check GitHub status**:
  - GET /api/github/status
  - Returns: { connected: true, username: "bartoszclapinski" }

**✅ Test**:
- After connecting, see "GitHub Connected: bartoszclapinski"
- Disconnect button works

**Time Estimate**: 2-3 hours

---

## Phase 2.3: GitHub Repositories Sync (Day 3-4)

**Goal**: Fetch and store user's GitHub repositories

### Step 2.3.1: Create Repository Service ✅

- [x] **Create `IGitHubRepositoryService` interface**:
  ```csharp
  Task<IEnumerable<GitHubRepositoryDto>> GetUserRepositoriesAsync(string accessToken, CancellationToken cancellationToken);
  ```

- [x] **Implement `GitHubRepositoryService`**:
  - Use Octokit.NET library for GitHub API
  - Fetch repositories using authenticated client
  - Map GitHub repo data to our Repository entity
  - Handle pagination (user might have many repos)

- [x] **Add Octokit.NET package**:
  ```powershell
  dotnet add src/DevMetricsPro.Infrastructure package Octokit
  ```

**✅ Test**: Call service method - should return list of repositories ✅

---

### Step 2.3.2: Create API Endpoint ✅

- [x] **Add to `GitHubController`**:
  - POST /api/github/sync-repositories - Trigger sync

- [x] **Implement sync logic**:
  - Get user's GitHub token from database
  - Call GitHubRepositoryService
  - Save to Repository table (upsert logic)
  - Return synced repository count and data

**✅ Test**:
- Call sync endpoint ✅
- Check database - repositories should be saved ✅
- Verified 36 repositories synced from GitHub ✅

---

### Step 2.3.3: Add Repositories Page ✅ (Completed Oct 31, 2025)

- [x] **Create `Components/Pages/Repositories.razor`**: ✅
  - Display list of synced repositories
  - Show: name, description, language, stars, last updated
  - Add "Sync Now" button
  - Add loading spinner during sync
  - Implement all UI states (loading, error, empty, not connected)
  - Use responsive MudGrid layout (3-column on desktop)
  - Sort by stars (most popular first)

**✅ Test**:
- Navigate to /repositories ✅
- See list of 36 GitHub repositories ✅
- Sync button fetches latest ✅
- Loading states work correctly ✅
- No console errors ✅

**Time Spent**: 3 hours  
**Status**: Complete ✅

---

## Phase 2.4: Commits Sync (Day 5-6)

**Goal**: Fetch and store commits for each repository

### Step 2.4.1: Create Commits Service

- [ ] **Create `IGitHubCommitsService`**:
  ```csharp
  Task<IEnumerable<CommitDto>> GetRepositoryCommitsAsync(string owner, string repo, string accessToken);
  Task SyncCommitsForRepositoryAsync(Guid repositoryId, CancellationToken cancellationToken);
  Task SyncAllCommitsAsync(Guid userId, CancellationToken cancellationToken);
  ```

- [ ] **Implement service**:
  - Fetch commits using Octokit
  - Map to Commit entity
  - Handle pagination (repos can have thousands of commits)
  - Store author info (link to Developer if exists)
  - Store commit metadata (additions, deletions, files changed)

**✅ Test**: Service method returns commits for a repository

---

### Step 2.4.2: Add Sync Logic

- [ ] **Implement incremental sync**:
  - Track last sync date per repository
  - Only fetch commits since last sync
  - Avoid duplicate commits (check SHA)

- [ ] **Add to `GitHubController`**:
  - POST /api/github/commits/sync/{repositoryId}
  - POST /api/github/commits/sync-all

**✅ Test**:
- Sync commits for one repository
- Check database - commits should be saved
- Re-sync - should only fetch new commits

---

### Step 2.4.3: Display Commit Activity

- [ ] **Update `Home.razor` dashboard**:
  - Show recent commits feed
  - Display: commit message, author, date, repo
  - Add commit count stat card

**✅ Test**:
- Dashboard shows recent commits
- Stats update after sync

**Time Estimate**: 4-5 hours

---

# WEEK 2: Background Jobs & Metrics

## Phase 2.5: Hangfire Setup (Day 7)

**Goal**: Configure background job processing

### Step 2.5.1: Install and Configure Hangfire

- [ ] **Add Hangfire packages**:
  ```powershell
  dotnet add src/DevMetricsPro.Web package Hangfire.Core
  dotnet add src/DevMetricsPro.Web package Hangfire.AspNetCore
  dotnet add src/DevMetricsPro.Web package Hangfire.PostgreSql
  ```

- [ ] **Configure in `Program.cs`**:
  - Add Hangfire services with PostgreSQL storage
  - Configure recurring jobs
  - Add Hangfire dashboard (/hangfire)

- [ ] **Secure Hangfire dashboard**:
  - Add authorization filter (only authenticated users)

**✅ Test**:
- Navigate to /hangfire
- Dashboard loads with jobs list

---

### Step 2.5.2: Create Background Jobs

- [ ] **Create `Jobs/SyncGitHubDataJob.cs`**:
  - Method: Execute(Guid userId)
  - Sync repositories → commits → PRs
  - Handle errors gracefully
  - Log progress

- [ ] **Schedule recurring job**:
  - Run every hour for all connected users
  - Configurable schedule in appsettings

**✅ Test**:
- Trigger job manually from Hangfire dashboard
- Verify data syncs correctly
- Check logs for errors

**Time Estimate**: 3 hours

---

## Phase 2.6: Pull Requests Sync (Day 8)

**Goal**: Fetch and store pull requests

### Step 2.6.1: Create PR Service

- [ ] **Create `IGitHubPullRequestService`**:
  - Fetch PRs for repository
  - Map to PullRequest entity
  - Store PR metadata (status, reviews, comments)

- [ ] **Implement service using Octokit**:
  - Get open and closed PRs
  - Link to repository and author

**✅ Test**: Service returns PRs for a repository

---

### Step 2.6.2: Add to Background Job

- [ ] **Update `SyncGitHubDataJob`**:
  - Add PR sync step
  - Sync after commits

- [ ] **Add API endpoints**:
  - GET /api/github/pull-requests
  - POST /api/github/pull-requests/sync

**✅ Test**:
- Background job syncs PRs
- API endpoint works

**Time Estimate**: 3 hours

---

## Phase 2.7: Basic Metrics Calculation (Day 9)

**Goal**: Calculate and store developer metrics

### Step 2.7.1: Create Metrics Service

- [ ] **Create `IMetricsCalculationService`**:
  ```csharp
  Task CalculateMetricsForDeveloperAsync(Guid developerId, DateTime startDate, DateTime endDate);
  Task CalculateMetricsForAllDevelopersAsync();
  ```

- [ ] **Implement basic metrics**:
  - Commit count per developer
  - Lines added/removed
  - PR count (opened, merged)
  - Active days (days with at least 1 commit)

- [ ] **Store in Metrics table**:
  - Link to Developer
  - Store MetricType and Value
  - Store date range

**✅ Test**:
- Calculate metrics for one developer
- Verify values in database
- Check calculations are correct

---

### Step 2.7.2: Add to Background Job

- [ ] **Update `SyncGitHubDataJob`**:
  - Add metrics calculation step
  - Run after data sync

- [ ] **Schedule separate metrics job**:
  - Run daily at midnight
  - Recalculate all metrics

**✅ Test**:
- Background job calculates metrics
- Dashboard shows updated metrics

**Time Estimate**: 4 hours

---

## Phase 2.8: Week 2 Wrap-up (Day 10)

**Goal**: Testing, cleanup, and documentation

### Step 2.8.1: End-to-End Testing

- [ ] **Test complete flow**:
  1. Connect GitHub account
  2. Sync repositories
  3. Background job runs
  4. Commits and PRs synced
  5. Metrics calculated
  6. Dashboard shows data

- [ ] **Test edge cases**:
  - Large repositories (1000+ commits)
  - Rate limiting scenarios
  - Network failures
  - Invalid tokens

**✅ Test**: Complete flow works without errors

---

### Step 2.8.2: Performance Optimization

- [ ] **Add caching**:
  - Cache repository lists (Redis)
  - Cache metrics calculations
  - Set appropriate TTL

- [ ] **Optimize queries**:
  - Add indexes for foreign keys
  - Use AsNoTracking() for read queries
  - Batch insert for commits

**✅ Test**: Performance improvements visible

---

### Step 2.8.3: Documentation

- [ ] **Update sprint-log.md**:
  - Document all learnings
  - Note challenges and solutions
  - Update metrics

- [ ] **Update README**:
  - Add GitHub integration section
  - Document OAuth setup
  - Add environment variables needed

- [ ] **Create ADRs** (Architecture Decision Records):
  - Why Hangfire for background jobs
  - Why Octokit for GitHub API
  - Token storage strategy

**✅ Test**: Documentation is complete and accurate

**Time Estimate**: 2-3 hours

---

## 📈 Sprint Success Criteria

- [x] GitHub OAuth working ✅
- [ ] Tokens stored securely in database
- [ ] Repositories synced from GitHub
- [ ] Commits synced in background
- [ ] Pull requests synced
- [ ] Hangfire configured and running
- [ ] Basic metrics calculated
- [ ] Dashboard displays real data
- [ ] Background jobs run reliably
- [ ] >80% test coverage
- [ ] Documentation updated

---

## 🔄 Sprint Retrospective Template

### What went well ✅
- 
- 

### What could be improved 🔄
- 
- 

### Technical debt identified 📝
- 
- 

### Action items for Sprint 3 🎯
- 
- 

---

## 🚀 Ready for Sprint 3?

Sprint 3 will focus on:
- Real-time dashboard with SignalR
- Interactive charts and visualizations
- Team leaderboards
- Contribution heatmaps
- Advanced metrics

---

**Last Updated**: October 28, 2025  
**Current Phase**: 2.1 Complete ✅  
**Next Phase**: 2.2 - Store GitHub Tokens

