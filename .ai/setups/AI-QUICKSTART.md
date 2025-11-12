# 🚀 AI Quick Start Guide

> **TL;DR**: Get a new AI assistant up to speed in 5 minutes.

---

## 📚 Read These Files First (In Order)

1. **`.ai/sprints/sprint1/sprint-log.md`** ← Start here! (Shows what's done, what's next)
2. **`.ai/WORKFLOW-GUIDE.md`** ← How we work (AI implements, user reviews)
3. **`.cursor/rules.md`** ← Core principles and conventions
4. **`.ai/prd.md`** ← What we're building

---

## ⚡ Current Status (As of Nov 11, 2025)

- **Sprint**: Sprint 2 - GitHub Integration
- **Progress**: Phase 2.6 complete ✅ (All 4 sub-phases!) + Hangfire working ✅
- **Next Phase**: Phase 2.7 - Basic Metrics Calculation
- **Branch**: Currently on `master` (all features merged!)

### ✅ What's Working:
- **Sprint 1 Complete**: Authentication, JWT, Blazor UI ✅
- **Sprint 2 Phase 2.1**: GitHub OAuth integration ✅
  - GitHub OAuth DTOs (Request, Response, Callback)
  - GitHubOAuthService with token exchange
  - GitHubController with authorize and callback endpoints
  - "Connect GitHub" button in Home page
  - Full OAuth flow tested successfully!
- **Sprint 2 Phase 2.2**: GitHub token storage ✅
  - GitHub fields added to ApplicationUser entity
  - Token, username, userId stored in database
  - Connection status displayed in UI
- **Sprint 2 Phase 2.3**: GitHub repository sync ✅
  - GitHubRepositoryService with Octokit.NET
  - POST /api/github/sync-repositories endpoint
  - Upsert logic (add new, update existing repos)
  - 36 repositories synced and persisted to PostgreSQL!
  - UI page displaying all repositories at `/repositories`
- **Sprint 2 Phase 2.4**: GitHub commits sync ✅
  - GitHubCommitsService with Octokit.NET
  - POST /api/github/commits/sync/{repositoryId} endpoint
  - GET /api/github/commits/recent endpoint
  - Sync Commits button on each repository
  - Dashboard shows total commit count
  - Incremental sync (only fetch new commits)
  - Developer entity creation with duplicate prevention
- **Sprint 2 Phase 2.5**: Hangfire Background Jobs ✅
  - Hangfire installed with PostgreSQL storage
  - Dashboard at /hangfire for job monitoring
  - SyncGitHubDataJob for automated syncing
  - POST /api/github/sync-all endpoint
  - Background job syncs repos + commits automatically
- **Sprint 2 Phase 2.6**: Pull Requests Sync ✅ (4 sub-phases!)
  - Phase 2.6.1: GitHubPullRequestDto + IGitHubPullRequestService
  - Phase 2.6.2: GitHubPullRequestService implementation with Octokit
  - Phase 2.6.3: POST /api/github/pull-requests/sync/{repositoryId} endpoint
  - Phase 2.6.4: SyncGitHubDataJob updated to sync PRs
  - Full PR sync: Open, Closed, Merged status
  - Developer auto-creation for PR authors
- **Sprint 2 UI Redesign**: Professional design system ✅
  - Custom design-system.css with CSS variables
  - Design tokens (colors, typography, spacing, shadows)
  - New layout components (TopNav, ControlPanel)
  - Reusable components (MetricCard, DataPanel, DataTable, StatusBadge)
  - Home.razor and Repositories.razor redesigned
  - Responsive design (desktop, tablet, mobile)
  - WCAG AAA compliant color palette

### ⏭️ What's Next:
- Phase 2.7: Basic metrics calculation (commit count, lines added/removed, PR count, etc.)
- Phase 2.8: Week 2 wrap-up and Sprint 2 completion
- Sprint 3: Real-time dashboard with SignalR, charts, visualizations

---

## 🎯 Key Rules

1. **Always create GitHub issue first** for each phase
2. **Always create feature branch** (never commit to master)
3. **AI guides, user implements, AI reviews** (hands-on learning)
4. **Explain as you code** (user is learning Blazor/.NET)
5. **Update sprint log** after completing work
6. **Ask for approval** before pushing

---

## 🏗️ Architecture

```
Core ← Application ← Infrastructure
                   ← Web
```

- **Core**: Domain entities, interfaces
- **Application**: Business logic, DTOs, services
- **Infrastructure**: EF Core, repositories, external services
- **Web**: Blazor UI, API controllers, SignalR hubs

**Rule**: Dependencies ONLY point inward!

---

## 🧰 Tech Stack

- .NET 9 + C# 12
- Blazor Server (SignalR)
- ASP.NET Core Identity + JWT
- PostgreSQL 16 + Redis 7
- Entity Framework Core 9
- MudBlazor

---

## 📁 Project Structure

```
.ai/
├── prd.md                    # Product requirements
├── WORKFLOW-GUIDE.md         # How we work
├── AI-ONBOARDING-PROMPT.md   # Full onboarding guide
└── sprints/sprint1/
    ├── sprint-plan.md        # Sprint plan
    └── sprint-log.md         # Daily progress ← READ THIS!

src/
├── DevMetricsPro.Core/       # Entities, interfaces
├── DevMetricsPro.Application/ # Services, DTOs
├── DevMetricsPro.Infrastructure/ # EF Core, repos
└── DevMetricsPro.Web/        # Blazor + API
    ├── Components/Pages/     # Login, Register, Home
    ├── Components/Layout/    # MainLayout, NavMenu
    ├── Controllers/          # AuthController
    └── Services/             # AuthStateService
```

---

## 💻 Common Commands

```powershell
# Start dev server
dotnet run --project src/DevMetricsPro.Web

# Start Docker (PostgreSQL + Redis)
docker-compose up -d

# Test auth endpoints
.\.ai\helpers\test-auth-endpoints.ps1

# Build solution
dotnet build

# Run tests
dotnet test
```

---

## ✅ Checklist Before Starting

- [ ] Read `sprint-log.md` - Know what's complete
- [ ] Read `WORKFLOW-GUIDE.md` - Understand workflow
- [ ] Confirm current branch (should be on feature branch)
- [ ] Verify Docker running (`docker ps`)
- [ ] Verify solution builds (`dotnet build`)

---

## 🎓 Learning Mode

User is learning Blazor/.NET, so:
- ✅ Explain concepts (What? Why? How?)
- ✅ Reference `.cursor/*.mdc` files for details
- ✅ Document learnings in sprint log
- ✅ Encourage questions

---

## 📝 Workflow Summary

1. **User asks for next phase**
2. **AI creates GitHub issue** (title, description, acceptance criteria)
3. **AI creates feature branch** (`feature/phase-X-Y`)
4. **AI provides implementation guidance** (detailed guidance, explaining concepts)
5. **User implements code in IDE** (asks questions, follows guidance)
6. **AI reviews implementation** (provides feedback, suggests improvements)
7. **AI updates sprint log** (what was done, learnings, time spent)
8. **User commits and pushes** to feature branch (with AI approval)
9. **User creates PR and merges** on GitHub
10. **Repeat** for next phase

---

## 🚀 First Message to Send

After reading this, confirm you understand:

```
I've reviewed the project status:
- Sprint 1: Complete ✅ (Authentication working!)
- Sprint 2: Phases 2.1-2.4 complete ✅ + UI Redesign complete ✅
  - Phase 2.1: GitHub OAuth ✅
  - Phase 2.2: Token storage ✅  
  - Phase 2.3: Repository sync ✅ (36 repos synced!)
  - Phase 2.4: Commits sync ✅ (working perfectly!)
  - UI Redesign: Complete ✅ (all 4 parts done!)
- Current branch: master (all merged!)
- Recent fixes:
  - Fixed commit count display on dashboard
  - Added "Sync Commits" button to repositories
  - Fixed duplicate developer constraint violation
  - Fixed incremental sync timing issue

I see we've completed a major milestone:
✅ GitHub integration fully working (OAuth, repos, commits)
✅ Professional UI redesign complete
✅ Dashboard shows real data from GitHub
✅ All features tested and working

Would you like to:
1. Continue with Phase 2.5 (Hangfire background jobs)?
2. Work on something else?
3. Review/improve existing features?

I'm ready to implement, explain, and keep docs updated! 🎯
```

---

**Pro Tip**: When stuck, check `sprint-log.md` - it has solutions to common problems we've already solved!

---

**Last Updated**: November 4, 2025  
**Version**: 2.4

