# DevMetrics Pro - Project Structure Reference

**Last Updated**: December 2, 2025  
**Purpose**: Complete project structure to check BEFORE implementing any new features to avoid duplication

---

## 📂 Solution Structure

```
DevMetricsPRO/
├── src/
│   ├── DevMetricsPro.Core/           # Domain Layer (No dependencies)
│   ├── DevMetricsPro.Application/    # Business Logic (depends on Core)
│   ├── DevMetricsPro.Infrastructure/ # Data & External Services (depends on Application + Core)
│   └── DevMetricsPro.Web/            # Blazor UI (depends on all layers)
├── tests/
│   ├── DevMetricsPro.Core.Tests/
│   ├── DevMetricsPro.Application.Tests/
│   └── DevMetricsPro.Integration.Tests/
└── .ai/                              # Project documentation
```

---

## 🔵 Core Layer (Domain)

**Path**: `src/DevMetricsPro.Core/`  
**Dependencies**: None (Pure domain logic)

### Entities

| File | Description |
|------|-------------|
| `Entities/BaseEntity.cs` | Base class with Id, CreatedAt, UpdatedAt, IsDeleted |
| `Entities/ApplicationUser.cs` | User entity (extends IdentityUser<Guid>) with GitHub fields |
| `Entities/Developer.cs` | Developer entity with metrics and repositories |
| `Entities/Repository.cs` | Repository entity with commits, PRs, GitHub metadata |
| `Entities/Commit.cs` | Commit entity with author, stats, files changed |
| `Entities/PullRequest.cs` | Pull request entity with status, reviews |
| `Entities/Metric.cs` | Metric entity for storing calculated metrics |

### Enums

| File | Values | Description |
|------|--------|-------------|
| `Enums/PlatformType.cs` | `GitHub`, `GitLab`, `AzureDevOps` | Source control platforms |
| `Enums/MetricType.cs` | `CommitCount`, `LinesAdded`, `LinesRemoved`, `PRCount`, etc. | Types of metrics |
| `Enums/PullRequestStatus.cs` | `Open`, `Closed`, `Merged`, `Draft` | PR statuses |

### Interfaces

| File | Description |
|------|-------------|
| `Interfaces/IRepository.cs` | Generic repository interface with CRUD operations |
| `Interfaces/IUnitOfWork.cs` | Unit of Work pattern interface for transactions |

---

## 🟢 Application Layer (Business Logic)

**Path**: `src/DevMetricsPro.Application/`  
**Dependencies**: Core

### DTOs - Authentication

| File | Purpose | Properties |
|------|---------|------------|
| `DTOs/Auth/LoginRequest.cs` | Login request | Email, Password, RememberMe |
| `DTOs/Auth/RegisterRequest.cs` | Registration request | Email, Password, ConfirmPassword |
| `DTOs/Auth/AuthResponse.cs` | Auth response | Token, Email, DisplayName, ExpiresAt, RefreshToken |

### DTOs - GitHub Integration

| File | Purpose | Properties |
|------|---------|------------|
| `DTOs/GitHub/GitHubOAuthRequest.cs` | OAuth request | ClientId, Scopes, State |
| `DTOs/GitHub/GitHubOAuthResponse.cs` | OAuth response | AccessToken, TokenType, GitHubUsername, GitHubUserId |
| `DTOs/GitHub/GitHubCallbackRequest.cs` | OAuth callback | Code, State |
| `DTOs/GitHub/GitHubRepositoryDto.cs` | **✅ REPOSITORY DATA** | Id, Name, Description, HtmlUrl, FullName, IsPrivate, IsFork, StargazersCount, ForksCount, OpenIssuesCount, Language, CreatedAt, UpdatedAt, PushedAt |
| `DTOs/GitHub/GitHubCommitDto.cs` | **✅ COMMIT DATA** | Sha, Message, AuthorName, AuthorEmail, CommitterName, CommitterEmail, CommittedAt, AuthorDate, HtmlUrl, Additions, Deletions, TotalChanges, RepositoryName |
| `DTOs/GitHub/GitHubPullRequestDto.cs` | **✅ PULL REQUEST DATA** | Number, Title, State, Body, HtmlUrl, AuthorLogin, AuthorName, CreatedAt, UpdatedAt, ClosedAt, MergedAt, IsMerged, IsDraft, Additions, Deletions, ChangedFiles, RepositoryName |

### DTOs - Charts (Sprint 3!) 📊

| File | Purpose | Properties |
|------|---------|------------|
| `DTOs/Charts/CommitActivityChartDto.cs` | **✅ COMMIT CHART DATA** | Labels, Values, TotalCommits, AveragePerDay, StartDate, EndDate |
| `DTOs/Charts/PullRequestChartDto.cs` | **✅ PR CHART DATA** | Labels, Values, TotalPRs, AverageReviewTimeHours, StartDate, EndDate |
| `DTOs/Charts/ContributionHeatmapDto.cs` | **✅ HEATMAP DATA** | Days, MaxContributions, TotalContributions, StartDate, EndDate |

### DTOs - Leaderboard & Metrics (Sprint 3!) 📊

| File | Purpose | Properties |
|------|---------|------------|
| `DTOs/LeaderboardEntryDto.cs` | **✅ LEADERBOARD ENTRY** | Rank, DeveloperId, DeveloperName, AvatarUrl, Value, Change, TrendDirection |
| `DTOs/SyncResultDto.cs` | **✅ SYNC RESULT** | RepositoriesSynced, CommitsSynced, PullRequestsSynced, MetricsCalculated, Success |
| `DTOs/Metrics/ReviewTimeMetricsDto.cs` | **✅ PR REVIEW METRICS** | AverageTimeToMergeHours, MedianTimeToMergeHours, MergeRatePercent, TotalPRsAnalyzed |
| `DTOs/Metrics/CodeVelocityDto.cs` | **✅ CODE VELOCITY** | WeeklyData, AverageCommitsPerWeek, AverageLinesPerWeek, CommitTrend |

### Enums - Application

| File | Values | Description |
|------|--------|-------------|
| `Enums/LeaderboardMetric.cs` | `Commits`, `PullRequests`, `LinesChanged`, `ActiveDays` | Leaderboard ranking metrics |

**⚠️ IMPORTANT**: 
- Use existing DTOs for all UI and API interactions!
- Check this list BEFORE creating new DTOs!

### Service Interfaces

| File | Methods | Description |
|------|---------|-------------|
| `Interfaces/IJwtService.cs` | `GenerateToken()`, `GenerateRefreshToken()` | JWT token generation |
| `Interfaces/IGitHubOAuthService.cs` | `GetAuthorizationUrl()`, `ExchangeCodeForTokenAsync()` | GitHub OAuth flow |
| `Interfaces/IGitHubRepositoryService.cs` | `GetUserRepositoriesAsync()` | Fetch repos from GitHub API |
| `Interfaces/IGitHubCommitsService.cs` | `GetRepositoryCommitsAsync()` | Fetch commits from GitHub API |
| `Interfaces/IGitHubPullRequestService.cs` | `GetRepositoryPullRequestsAsync()` | Fetch PRs from GitHub API |
| `Interfaces/IMetricsCalculationService.cs` | `CalculateMetricsForDeveloperAsync()`, `GetReviewTimeMetricsAsync()`, `GetCodeVelocityAsync()` | Calculate developer metrics |
| `Interfaces/IChartDataService.cs` | `GetCommitActivityAsync()`, `GetPullRequestStatsAsync()`, `GetContributionHeatmapAsync()` | Chart data aggregation |
| `Interfaces/ILeaderboardService.cs` | `GetLeaderboardAsync()` | Leaderboard data |
| `Interfaces/IMetricsHubService.cs` | `NotifyMetricsUpdatedAsync()`, `NotifySyncCompletedAsync()`, `NotifySyncStartedAsync()` | SignalR notifications |

### Services (Application Layer)

| File | Implements | Description |
|------|------------|-------------|
| `Services/ChartDataService.cs` | `IChartDataService` | Aggregates data for charts |
| `Services/LeaderboardService.cs` | `ILeaderboardService` | Ranks developers by metric |

---

## 🟡 Infrastructure Layer (Data & Services)

**Path**: `src/DevMetricsPro.Infrastructure/`  
**Dependencies**: Application, Core

### Data - Database

| File | Description |
|------|-------------|
| `Data/ApplicationDbContext.cs` | EF Core DbContext with Identity integration |
| `Data/DbInitializer.cs` | Seed data for development (3 test developers) |

### Data - Entity Configurations

| File | Configures |
|------|------------|
| `Data/Configurations/DeveloperConfiguration.cs` | Developer entity mapping |
| `Data/Configurations/RepositoryConfiguration.cs` | Repository entity mapping |
| `Data/Configurations/CommitConfiguration.cs` | Commit entity mapping |
| `Data/Configurations/PullRequestConfiguration.cs` | PullRequest entity mapping |
| `Data/Configurations/MetricConfiguration.cs` | Metric entity mapping |

### Repositories

| File | Implements |
|------|------------|
| `Repositories/Repository.cs` | Generic repository pattern (IRepository<T>) |
| `Repositories/UnitOfWork.cs` | Unit of Work pattern (IUnitOfWork) |

### Services (Infrastructure Layer)

| File | Implements | Description |
|------|------------|-------------|
| `Services/JwtService.cs` | `IJwtService` | JWT token generation with claims |
| `Services/GitHubOAuthService.cs` | `IGitHubOAuthService` | GitHub OAuth token exchange |
| `Services/GitHubRepositoryService.cs` | `IGitHubRepositoryService` | Fetch repos using Octokit.NET |
| `Services/GitHubCommitsService.cs` | `IGitHubCommitsService` | Fetch commits using Octokit.NET |
| `Services/GitHubPullRequestService.cs` | `IGitHubPullRequestService` | Fetch PRs using Octokit.NET |
| `Services/MetricsCalculationService.cs` | `IMetricsCalculationService` | Calculate developer metrics including advanced metrics |

### Migrations

| Migration | Date | Description |
|-----------|------|-------------|
| `20251017120315_InitialDatabaseSchema` | Oct 17 | Initial entities (Developer, Repository, Commit, PR, Metric) |
| `20251020115958_AddIdentityTables` | Oct 20 | ASP.NET Core Identity tables |
| `20251030085317_AddGitHubFieldsToUser` | Oct 30 | GitHub integration fields (AccessToken, Username, UserId, ConnectedAt) |
| `20251030153948_AddGitHubFieldsToRepository` | Oct 30 | GitHub repository metadata (FullName, IsPrivate, IsFork, Stars, etc.) |

---

## 🔴 Web Layer (UI & API)

**Path**: `src/DevMetricsPro.Web/`  
**Dependencies**: Infrastructure, Application, Core

### Controllers (API Endpoints)

| File | Routes | Description |
|------|--------|-------------|
| `Controllers/AuthController.cs` | `/api/auth/register`, `/api/auth/login` | Authentication API |
| `Controllers/GitHubController.cs` | `/api/github/*` | GitHub integration API |

**Available API Endpoints:**
- ✅ `POST /api/auth/register` - Register new user
- ✅ `POST /api/auth/login` - Login user
- ✅ `GET /api/github/authorize` - Initiate OAuth flow
- ✅ `GET /api/github/callback` - OAuth callback
- ✅ `GET /api/github/status` - Check GitHub connection
- ✅ `POST /api/github/sync-repositories` - Sync repos from GitHub
- ✅ `POST /api/github/commits/sync/{repositoryId}` - Sync commits for repository
- ✅ `GET /api/github/commits/recent?limit=10` - Get recent commits
- ✅ `POST /api/github/pull-requests/sync/{repositoryId}` - Sync PRs for repository
- ✅ `GET /api/github/pull-requests?repositoryId={guid}&status={all|open|closed|merged}` - Get PRs from database
- ✅ `POST /api/github/sync-all` - Trigger full sync background job

### SignalR Hubs (NEW - Sprint 3!) 🔔

| File | Endpoint | Description |
|------|----------|-------------|
| `Hubs/MetricsHub.cs` | `/hubs/metrics` | Real-time dashboard updates |

**Hub Methods:**
- `JoinDashboard(userId)` - Subscribe to user updates
- `LeaveDashboard(userId)` - Unsubscribe from updates

**Events Sent:**
- `SyncStarted` - When data sync begins
- `SyncCompleted` - When data sync completes (includes stats)
- `MetricsUpdated` - When metrics are recalculated

### Blazor Pages

| File | Route | Purpose | Status |
|------|-------|---------|--------|
| `Components/Pages/Home.razor` | `/` | Dashboard with charts, heatmap, leaderboard, and real-time updates | ✅ Working |
| `Components/Pages/Login.razor` | `/login` | User login | ✅ Working |
| `Components/Pages/Register.razor` | `/register` | User registration | ✅ Working |
| `Components/Pages/Repositories.razor` | `/repositories` | Display synced GitHub repos (36+ repos) | ✅ Working |
| `Components/Pages/PullRequests.razor` | `/pull-requests` | Display PRs with filtering (repo, status) | ✅ Working |
| `Components/Pages/Test.razor` | `/test` | Test page for development | ✅ Working |
| `Components/Pages/Weather.razor` | `/weather` | Demo page | ✅ Working |
| `Components/Pages/Counter.razor` | `/counter` | Demo page | ✅ Working |
| `Components/Pages/Error.razor` | `/error` | Error page | ✅ Working |

### Layout Components

| File | Purpose | Status |
|------|---------|--------|
| `Components/Layout/MainLayout.razor` | Main app layout wrapper | ✅ Working |
| `Components/Layout/TopNav.razor` | Horizontal navigation bar with tabs | ✅ Working |
| `Components/Layout/ControlPanel.razor` | Filters and action buttons | ✅ Working |
| `Components/Layout/NavMenu.razor` | Legacy navigation menu | ✅ Working |

### Shared/Reusable Components

| File | Purpose | Status |
|------|---------|--------|
| `Components/Shared/MetricCard.razor` | Display metrics with value, label, trend | ✅ Working |
| `Components/Shared/DataPanel.razor` | Generic panel container with header | ✅ Working |
| `Components/Shared/DataTable.razor` | Generic table component with templates | ✅ Working |
| `Components/Shared/StatusBadge.razor` | Colored status indicators | ✅ Working |
| `Components/Shared/Leaderboard.razor` | **✅ NEW!** Team leaderboard with rankings | ✅ Working |
| `Components/Shared/Leaderboard.razor.css` | Scoped CSS for leaderboard | ✅ Working |

### Chart Components (Sprint 3!) 📊

| File | Purpose | Status |
|------|---------|--------|
| `Components/Shared/Charts/LineChart.razor` | Reusable line chart (Chart.js) | ✅ Working |
| `Components/Shared/Charts/LineChart.razor.css` | Scoped CSS for line chart | ✅ Working |
| `Components/Shared/Charts/BarChart.razor` | Reusable bar chart (Chart.js) | ✅ Working |
| `Components/Shared/Charts/BarChart.razor.css` | Scoped CSS for bar chart | ✅ Working |
| `Components/Shared/Charts/ContributionHeatmap.razor` | **✅ NEW!** GitHub-style heatmap | ✅ Working |
| `Components/Shared/Charts/ContributionHeatmap.razor.css` | Scoped CSS for heatmap | ✅ Working |

### JavaScript Files (Sprint 3!) 📊

| File | Purpose | Status |
|------|---------|--------|
| `wwwroot/js/charts.js` | Chart.js JSInterop wrapper | ✅ Working |

**Chart.js Functions:**
- `chartHelpers.createLineChart(canvasId, config)` - Create line chart
- `chartHelpers.createBarChart(canvasId, config)` - Create bar chart
- `chartHelpers.updateChart(canvasId, newData)` - Update chart data
- `chartHelpers.destroyChart(canvasId)` - Clean up chart instance

### Stylesheets

| File | Purpose | Status |
|------|---------|--------|
| `wwwroot/css/design-system.css` | Design tokens, color palette, core styles | ✅ Complete |

### Other Layout Files

| File | Purpose |
|------|---------|
| `Components/App.razor` | Root Blazor app component (includes Chart.js CDN) |
| `Components/Routes.razor` | Route configuration |
| `Components/_Imports.razor` | Global using statements for components |

### Services (Client-Side)

| File | Purpose |
|------|---------|
| `Services/AuthStateService.cs` | Manages JWT token in localStorage, checks authentication state, gets user ID |
| `Services/MetricsHubService.cs` | **✅ NEW!** Sends SignalR notifications to clients |
| `Services/SignalRService.cs` | **✅ NEW!** Client-side SignalR connection management |

### Background Jobs

| File | Purpose | Status |
|------|---------|--------|
| `Jobs/SyncGitHubDataJob.cs` | Background job for syncing GitHub data (repos, commits, PRs) | ✅ Working |

**What it does:**
- Syncs repositories from GitHub API
- Syncs commits for each repository (incremental)
- Syncs pull requests for each repository (incremental)
- Auto-creates Developer entities for contributors
- Uses `LastSyncedAt` for efficient incremental syncs
- **Sends SignalR notifications** on sync start/complete
- Triggered via: `POST /api/github/sync-all` endpoint
- Managed by Hangfire (dashboard at `/hangfire`)

### Middleware

| File | Purpose |
|------|---------|
| `Middleware/GlobalExceptionHandler.cs` | Global exception handling with logging |
| `Middleware/HangfireAuthorizationFilter.cs` | Hangfire dashboard authorization |

---

## 🎯 Current Implementation Status

### ✅ Completed (Sprint 0 & 1)

- [x] Clean Architecture setup
- [x] Database with PostgreSQL + EF Core
- [x] All domain entities and configurations
- [x] Repository pattern + Unit of Work
- [x] ASP.NET Core Identity + JWT authentication
- [x] Auth API endpoints (register/login)
- [x] Blazor UI with MudBlazor
- [x] Login/Register pages
- [x] Dashboard (Home) page
- [x] Logging with Serilog
- [x] Global exception handling

### ✅ Completed (Sprint 2)

- [x] Phase 2.1: GitHub OAuth integration ✅
- [x] Phase 2.2: Store GitHub tokens in database ✅
- [x] Phase 2.3: GitHub repositories sync ✅
- [x] Phase 2.4: Commits sync ✅
- [x] Phase 2.5: Hangfire setup ✅
- [x] Phase 2.6: Pull requests sync ✅
- [x] Phase 2.7: Basic metrics calculation ✅
- [x] UI Redesign: Professional design system ✅

### ✅ Completed (Sprint 3 - Charts & Real-time) ~80% Done!

- [x] Phase 3.1: Chart Library Setup ✅ (Chart.js via JSInterop)
- [x] Phase 3.2: Commit Activity Chart ✅ (Line chart with real data!)
- [x] Phase 3.3: PR Statistics Bar Chart ✅ (Bar chart with status breakdown)
- [x] Phase 3.4: Contribution Heatmap ✅ (GitHub-style calendar!)
- [x] Phase 3.5: Team Leaderboard ✅ (Sortable with metrics!)
- [x] Phase 3.6: SignalR Hub Setup ✅ (Real-time notifications!)
- [x] Phase 3.7: Client-Side SignalR ✅ (Auto-refresh dashboard!)
- [x] Phase 3.8: Advanced Metrics ✅ (PR review time, code velocity!)
- [ ] Phase 3.9: Time Range Filters (NEXT!)
- [ ] Phase 3.10: Polish & Performance

### ⏭️ Not Started

- Developers page (`/developers`)
- Metrics page (`/metrics`)
- Settings page (`/settings`)

---

## 📋 Quick Reference: When to Create New Files

### ❌ DON'T CREATE if exists:

**DTOs:**
- ✅ `GitHubRepositoryDto` - Use for ALL repository data
- ✅ `GitHubCommitDto` - Use for commit data
- ✅ `GitHubPullRequestDto` - Use for PR data
- ✅ `CommitActivityChartDto` - Use for commit charts
- ✅ `PullRequestChartDto` - Use for PR charts
- ✅ `ContributionHeatmapDto` - Use for heatmap
- ✅ `LeaderboardEntryDto` - Use for leaderboard
- ✅ `ReviewTimeMetricsDto` - Use for PR review metrics
- ✅ `CodeVelocityDto` - Use for velocity metrics
- ✅ `SyncResultDto` - Use for sync notifications
- ✅ `LoginRequest`, `RegisterRequest`, `AuthResponse` - Use for auth

**Services:**
- ✅ `IChartDataService` - Use for chart data aggregation
- ✅ `ILeaderboardService` - Use for leaderboard data
- ✅ `IMetricsCalculationService` - Use for all metrics
- ✅ `IMetricsHubService` - Use for SignalR notifications
- ✅ `IGitHubRepositoryService` - Use for fetching repos
- ✅ `IGitHubCommitsService` - Use for fetching commits
- ✅ `IGitHubPullRequestService` - Use for fetching PRs
- ✅ `IGitHubOAuthService` - Use for OAuth flow
- ✅ `IJwtService` - Use for JWT tokens
- ✅ `AuthStateService` - Use for client-side auth state
- ✅ `SignalRService` - Use for client-side SignalR

**Components:**
- ✅ `LineChart.razor` - Use for line charts
- ✅ `BarChart.razor` - Use for bar charts
- ✅ `ContributionHeatmap.razor` - Use for heatmaps
- ✅ `Leaderboard.razor` - Use for leaderboards
- ✅ `MetricCard.razor` - Use for metric display
- ✅ `DataPanel.razor` - Use for panel containers
- ✅ `DataTable.razor` - Use for tables

**Entities:**
- ✅ All 7 entities exist (Developer, Repository, Commit, PullRequest, Metric, ApplicationUser, BaseEntity)

### ✅ OK TO CREATE:

**Components (not yet implemented):**
- `TimeRangeSelector.razor` - For Phase 3.9

**Services (not yet implemented):**
- `DashboardStateService.cs` - For Phase 3.9

**Pages (not yet implemented):**
- `Developers.razor` - Display developers list
- `Metrics.razor` - Display metrics charts
- `Settings.razor` - User settings

---

## 🚨 Before Implementing ANYTHING:

### Step 1: Check This Document
- ✅ Does a DTO already exist for this data?
- ✅ Does a service interface already exist?
- ✅ Does a component already exist?
- ✅ Does a page already exist at this route?
- ✅ Does an entity already exist in Core?

### Step 2: Check Application Layer
```bash
ls src/DevMetricsPro.Application/DTOs/
ls src/DevMetricsPro.Application/Interfaces/
ls src/DevMetricsPro.Application/Services/
```

### Step 3: Check Web Layer
```bash
ls src/DevMetricsPro.Web/Components/Pages/
ls src/DevMetricsPro.Web/Components/Shared/
ls src/DevMetricsPro.Web/Components/Shared/Charts/
ls src/DevMetricsPro.Web/Controllers/
ls src/DevMetricsPro.Web/Hubs/
ls src/DevMetricsPro.Web/Services/
```

---

## 🎓 Clean Architecture Rules

### Dependencies Flow (CRITICAL)
```
Core (no dependencies)
  ↑
Application (depends on Core)
  ↑
Infrastructure (depends on Application + Core)
  ↑
Web (depends on Infrastructure + Application + Core)
```

### Layer Responsibilities

| Layer | Contains | Does NOT Contain |
|-------|----------|------------------|
| **Core** | Entities, Enums, Interfaces | Business logic, data access, external services |
| **Application** | DTOs, Service Interfaces, Business Logic | Data access implementation, external API calls |
| **Infrastructure** | DbContext, Repositories, Service Implementations | UI components, controllers |
| **Web** | Blazor Components, Controllers, UI Services, Hubs | Business logic, direct database access |

---

**Last Updated**: December 2, 2025 (Post Phase 3.8)  
**Current Sprint**: Sprint 3 - Charts & Real-time Dashboard  
**Current Phase**: Phase 3.9 - Time Range Filters (Next)  
**Progress**: 8/10 phases done (80%)  
**Next Review**: After Sprint 3 completion

