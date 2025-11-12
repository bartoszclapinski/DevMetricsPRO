# DevMetrics Pro - Project Structure Reference

**Last Updated**: October 31, 2025  
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
| `DTOs/GitHub/GitHubCommitDto.cs` | **✅ COMMIT DATA** (Phase 2.4) | Sha, Message, AuthorName, AuthorEmail, CommitterName, CommitterEmail, CommittedAt, AuthorDate, HtmlUrl, Additions, Deletions, TotalChanges, RepositoryName |
| `DTOs/GitHub/GitHubPullRequestDto.cs` | **✅ PULL REQUEST DATA** (Phase 2.6) | Number, Title, State, Body, HtmlUrl, AuthorLogin, AuthorName, CreatedAt, UpdatedAt, ClosedAt, MergedAt, IsMerged, IsDraft, Additions, Deletions, ChangedFiles, RepositoryName |

**⚠️ IMPORTANT**: 
- `GitHubRepositoryDto` is the **ONLY** repository DTO. Use this for all UI and API interactions!
- `GitHubCommitDto` is the **ONLY** commit DTO. Use this for all commit-related UI and API interactions!
- `GitHubPullRequestDto` is the **ONLY** PR DTO. Use this for all PR-related UI and API interactions!

### Service Interfaces

| File | Methods | Description |
|------|---------|-------------|
| `Interfaces/IJwtService.cs` | `GenerateToken()`, `GenerateRefreshToken()` | JWT token generation |
| `Interfaces/IGitHubOAuthService.cs` | `GetAuthorizationUrl()`, `ExchangeCodeForTokenAsync()` | GitHub OAuth flow |
| `Interfaces/IGitHubRepositoryService.cs` | `GetUserRepositoriesAsync()` | Fetch repos from GitHub API |
| `Interfaces/IGitHubCommitsService.cs` | `GetRepositoryCommitsAsync()` | Fetch commits from GitHub API (Phase 2.4) ✅ |
| `Interfaces/IGitHubPullRequestService.cs` | `GetRepositoryPullRequestsAsync()` | Fetch PRs from GitHub API (Phase 2.6) ✅ |

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

### Services

| File | Implements | Description |
|------|------------|-------------|
| `Services/JwtService.cs` | `IJwtService` | JWT token generation with claims |
| `Services/GitHubOAuthService.cs` | `IGitHubOAuthService` | GitHub OAuth token exchange |
| `Services/GitHubRepositoryService.cs` | `IGitHubRepositoryService` | Fetch repos using Octokit.NET |
| `Services/GitHubCommitsService.cs` | `IGitHubCommitsService` | Fetch commits using Octokit.NET (Phase 2.4) ✅ |
| `Services/GitHubPullRequestService.cs` | `IGitHubPullRequestService` | Fetch PRs using Octokit.NET (Phase 2.6) ✅ |

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
| `Controllers/GitHubController.cs` | `/api/github/authorize`, `/api/github/callback`, `/api/github/status`, `/api/github/sync-repositories` | GitHub integration API |

**Available API Endpoints:**
- ✅ `POST /api/auth/register` - Register new user
- ✅ `POST /api/auth/login` - Login user
- ✅ `GET /api/github/authorize` - Initiate OAuth flow
- ✅ `GET /api/github/callback` - OAuth callback
- ✅ `GET /api/github/status` - Check GitHub connection
- ✅ `POST /api/github/sync-repositories` - Sync repos from GitHub
- ✅ `POST /api/github/commits/sync/{repositoryId}` - Sync commits for repository (Phase 2.4) ✅
- ✅ `GET /api/github/commits/recent?limit=10` - Get recent commits (Phase 2.4) ✅
- ✅ `POST /api/github/pull-requests/sync/{repositoryId}` - Sync PRs for repository (Phase 2.6) ✅
- ✅ `GET /api/github/pull-requests?repositoryId={guid}&status={all|open|closed|merged}` - Get PRs from database (Phase 2.6.5) ✅
- ✅ `POST /api/github/sync-all` - Trigger full sync background job (Phase 2.5) ✅

### Blazor Pages

| File | Route | Purpose | Status |
|------|-------|---------|--------|
| `Components/Pages/Home.razor` | `/` | Dashboard with stats and GitHub connection | ✅ Working (Redesigned) |
| `Components/Pages/Login.razor` | `/login` | User login | ✅ Working |
| `Components/Pages/Register.razor` | `/register` | User registration | ✅ Working |
| `Components/Pages/Repositories.razor` | `/repositories` | Display synced GitHub repos (36 repos) | ✅ Working (Redesigned) |
| `Components/Pages/PullRequests.razor` | `/pull-requests` | Display PRs with filtering (repo, status) | ✅ Working (Phase 2.6.5) |
| `Components/Pages/Test.razor` | `/test` | Test page for development | ✅ Working |
| `Components/Pages/Weather.razor` | `/weather` | Demo page | ✅ Working |
| `Components/Pages/Counter.razor` | `/counter` | Demo page | ✅ Working |
| `Components/Pages/Error.razor` | `/error` | Error page | ✅ Working |

### Layout Components (NEW - Sprint 2)

| File | Purpose | Status |
|------|---------|--------|
| `Components/Layout/MainLayout.razor` | Main app layout wrapper | ✅ Working (Redesigned) |
| `Components/Layout/TopNav.razor` | Horizontal navigation bar with tabs | ✅ Working |
| `Components/Layout/ControlPanel.razor` | Filters and action buttons | ✅ Working |

### Shared/Reusable Components (NEW - Sprint 2)

| File | Purpose | Status |
|------|---------|--------|
| `Components/Shared/MetricCard.razor` | Display metrics with value, label, trend | ✅ Working |
| `Components/Shared/DataPanel.razor` | Generic panel container with header | ✅ Working |
| `Components/Shared/DataTable.razor` | Generic table component with templates | ✅ Working |
| `Components/Shared/StatusBadge.razor` | Colored status indicators | ✅ Working |

### Stylesheets (NEW - Sprint 2)

| File | Purpose | Status |
|------|---------|--------|
| `wwwroot/css/design-system.css` | Design tokens, color palette, core styles | ✅ Complete |

**📝 Note**: `/developers`, `/metrics`, `/settings` pages not yet created

### Other Layout Files

| File | Purpose |
|------|---------|
| `Components/Layout/NavMenu.razor` | Legacy navigation menu (may be removed) |
| `Components/App.razor` | Root Blazor app component |
| `Components/Routes.razor` | Route configuration |
| `Components/_Imports.razor` | Global using statements for components |

### Services (Client-Side)

| File | Purpose |
|------|---------|
| `Services/AuthStateService.cs` | Manages JWT token in localStorage, checks authentication state |

### Background Jobs (Phase 2.5)

| File | Purpose | Status |
|------|---------|--------|
| `Jobs/SyncGitHubDataJob.cs` | Background job for syncing GitHub data (repos, commits, PRs) | ✅ Working |

**What it does:**
- Syncs repositories from GitHub API
- Syncs commits for each repository (incremental)
- Syncs pull requests for each repository (incremental)
- Auto-creates Developer entities for contributors
- Uses `LastSyncedAt` for efficient incremental syncs
- Triggered via: `POST /api/github/sync-all` endpoint
- Managed by Hangfire (dashboard at `/hangfire`)

### Middleware

| File | Purpose |
|------|---------|
| `Middleware/GlobalExceptionHandler.cs` | Global exception handling with logging |
| `Middleware/HangfireAuthorizationFilter.cs` | Hangfire dashboard authorization (DEBUG: open, RELEASE: auth required) |

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

### 🚧 In Progress (Sprint 2)

- [x] Phase 2.1: GitHub OAuth integration ✅
- [x] Phase 2.2: Store GitHub tokens in database ✅
- [x] Phase 2.3: GitHub repositories sync (backend) ✅
- [x] Phase 2.3.3: Repositories UI page ✅
- [x] Phase 2.4: Commits sync ✅
- [x] Phase 2.5: Hangfire setup ✅
- [x] Phase 2.6: Pull requests sync ✅
  - [x] Phase 2.6.1: PR DTOs & Interface ✅
  - [x] Phase 2.6.2: PR Service Implementation ✅
  - [x] Phase 2.6.3: PR API Endpoint ✅
  - [x] Phase 2.6.4: PR Background Job Integration ✅
- [ ] Phase 2.7: Basic metrics calculation (Next!)

### ⏭️ Not Started

- Developers page (`/developers`)
- Metrics page (`/metrics`)
- Settings page (`/settings`)
- Pull Requests UI page (`/pull-requests`)
- Metrics calculation service
- Redis caching (Phase 2.8)
- Dashboard charts (ApexCharts) (Sprint 3)
- SignalR real-time updates (Sprint 3)

---

## 📋 Quick Reference: When to Create New Files

### ❌ DON'T CREATE if exists:

**DTOs:**
- ✅ `GitHubRepositoryDto` - Use for ALL repository data
- ✅ `LoginRequest`, `RegisterRequest`, `AuthResponse` - Use for auth
- ✅ `GitHubOAuthRequest`, `GitHubOAuthResponse` - Use for OAuth

**Services:**
- ✅ `IGitHubRepositoryService` - Use for fetching repos
- ✅ `IGitHubOAuthService` - Use for OAuth flow
- ✅ `IJwtService` - Use for JWT tokens
- ✅ `AuthStateService` - Use for client-side auth state

**Entities:**
- ✅ All 7 entities exist (Developer, Repository, Commit, PullRequest, Metric, ApplicationUser, BaseEntity)

### ✅ OK TO CREATE:

**Services (not yet implemented):**
- `IMetricsCalculationService` - For Phase 2.7

**Pages (not yet implemented):**
- `Developers.razor` - Display developers list
- `PullRequests.razor` - Display synced PRs
- `Metrics.razor` - Display metrics charts
- `Settings.razor` - User settings

**UI Response Wrappers:**
- Internal classes in `.razor` files for API responses (not business DTOs)
- Example: `SyncRepositoriesResponse`, `GitHubStatusResponse`

---

## 🚨 Before Implementing ANYTHING:

### Step 1: Check This Document
- ✅ Does a DTO already exist for this data?
- ✅ Does a service interface already exist?
- ✅ Does a page already exist at this route?
- ✅ Does an entity already exist in Core?

### Step 2: Check Application Layer
```bash
ls src/DevMetricsPro.Application/DTOs/
ls src/DevMetricsPro.Application/Interfaces/
```

### Step 3: Check Infrastructure Layer
```bash
ls src/DevMetricsPro.Infrastructure/Services/
```

### Step 4: Check Web Layer
```bash
ls src/DevMetricsPro.Web/Components/Pages/
ls src/DevMetricsPro.Web/Controllers/
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
| **Web** | Blazor Components, Controllers, UI Services | Business logic, direct database access |

---

## 💡 Common Patterns

### API Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class MyController : ControllerBase
{
    private readonly IMyService _service;
    private readonly ILogger<MyController> _logger;
    
    // Constructor injection
    // Methods with [HttpGet], [HttpPost], etc.
}
```

### Blazor Page Pattern
```razor
@page "/mypage"
@using DevMetricsPro.Application.DTOs.MyDtos
@inject HttpClient Http
@inject AuthStateService AuthState
@inject NavigationManager Navigation
@inject ILogger<MyPage> Logger
@inject ISnackbar Snackbar

<PageTitle>My Page</PageTitle>

<!-- UI -->

@code {
    // State
    // OnInitializedAsync()
    // Methods
}
```

### Service Pattern
```csharp
public class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public async Task<Result> DoSomethingAsync(
        string param, 
        CancellationToken cancellationToken = default)
    {
        // Implementation
    }
}
```

---

## 📞 Questions to Ask Before Creating New Files

1. **DTOs**: Does Application layer have this DTO already?
2. **Services**: Does an interface exist in Application/Interfaces?
3. **Pages**: Check `NavMenu.razor` - is the route already defined?
4. **Entities**: All 7 core entities exist - do I need a new one?
5. **Controllers**: Does a controller for this resource exist?

---

**Last Updated**: November 11, 2025 (Post Phase 2.6)  
**Current Sprint**: Sprint 2 - GitHub Integration  
**Current Phase**: Phase 2.7 - Basic Metrics Calculation (Next)  
**Progress**: Week 2 In Progress - 6/8 phases done (75%)  
**Next Review**: After Sprint 2 completion


