# 🤖 AI Assistant Onboarding Prompt for DevMetrics Pro

> **Purpose**: This document provides a complete onboarding prompt for a new AI assistant to seamlessly continue development on DevMetrics Pro.

---

## 📋 Initial Prompt for New AI Session

Copy and use this prompt to start a new AI session:

```
Hi! I need your help continuing development on DevMetrics Pro, a real-time developer analytics dashboard built with .NET 9 and Blazor Server.

Before we start, please read these files IN THIS ORDER to understand the project:

1. **Current Sprint Status**:
   - Read `.ai/sprints/sprint3/sprint-log.md` - What's been done so far
   - Read `.ai/sprints/sprint3/sprint-plan.md` - Current sprint plan

2. **Project Overview & Tech Stack**:
   - Read `.ai/setups/prd.md` - Product Requirements Document
   - Read `README.md` - Quick project overview

3. **Development Workflow**:
   - Read `.ai/setups/WORKFLOW-GUIDE.md` - HOW we work together
   - Read `.ai/setups/PROJECT-STRUCTURE.md` - What exists in codebase

4. **Architecture & Conventions**:
   - Read `.cursor/rules.md` - Core principles and quick reference
   - When needed, refer to specific rule files:
     - `.cursor/architecture.mdc` - Clean Architecture details
     - `.cursor/dotnet-conventions.mdc` - C# and .NET standards
     - `.cursor/blazor-rules.mdc` - Blazor patterns
     - `.cursor/database-rules.mdc` - EF Core and database
     - `.cursor/testing-rules.mdc` - Testing standards

**Important**: After reading, please:
1. Confirm you understand the current sprint status
2. Tell me what phase we're on and what's next
3. Ask if I want to continue with the next planned phase or do something else

Let's continue building! 🚀
```

---

## 🎯 What the AI Should Do After Reading

After the AI reads those files, it should:

### 1. **Understand Current State**
- ✅ Know **Sprint 1** is **COMPLETE** (Authentication) ✅
- ✅ Know **Sprint 2** is **COMPLETE** (GitHub Integration & Background Jobs) ✅
- ✅ Know **Sprint 3** is **IN PROGRESS** at **80%** (Charts & Real-time Dashboard)
  - Phase 3.1: Chart Library Setup ✅
  - Phase 3.2: Commit Activity Chart ✅
  - Phase 3.3: PR Statistics Bar Chart ✅
  - Phase 3.4: Contribution Heatmap ✅
  - Phase 3.5: Team Leaderboard ✅
  - Phase 3.6: SignalR Hub Setup ✅
  - Phase 3.7: Client-Side SignalR ✅
  - Phase 3.8: Advanced Metrics ✅
  - Phase 3.9: Time Range Filters (NEXT)
  - Phase 3.10: Polish & Performance
- ✅ Know the **next phase** is **Phase 3.9** (Time Range Filters)

### 2. **Understand the Workflow**
- ✅ **AI provides guidance** on what to implement and how
- ✅ **AI can implement code** with user approval
- ✅ **User reviews** implementations and approves changes
- ✅ **Issue-driven development**: Create GitHub issue → branch → implement → PR → merge
- ✅ **Learning mode**: Explain concepts, teach as we go
- ✅ **Documentation**: Keep sprint log updated with learnings

### 3. **Know the Tech Stack**
- .NET 9 with C# 12
- Blazor Server (SignalR for real-time)
- ASP.NET Core Identity + JWT
- PostgreSQL 16 + TimescaleDB
- Redis 7
- Entity Framework Core 9
- MudBlazor for UI
- **Chart.js** for visualizations (via JSInterop)
- **SignalR** for real-time updates
- Hangfire for background jobs

### 4. **Follow the Architecture**
```
Core ← Application ← Infrastructure
                   ← Web
```
- Dependencies ONLY point inward
- Use async/await everywhere with CancellationToken
- Use DTOs for data transfer (never expose entities)
- Keep business logic in Application layer

### 5. **Key Conventions**
- ✅ File-scoped namespaces
- ✅ Nullable reference types enabled
- ✅ Async methods end with `Async`
- ✅ Constructor injection for DI
- ✅ PascalCase for classes/methods, _camelCase for private fields
- ✅ Use `required` keyword for required properties
- ✅ Use modern C# 12 features (collection expressions, etc.)

---

## 📁 Key Files/Directories to Know

### Documentation (Read First!)
```
.ai/
├── setups/
│   ├── prd.md                    # Product requirements
│   ├── WORKFLOW-GUIDE.md         # HOW we work
│   ├── PROJECT-STRUCTURE.md      # Complete codebase map
│   └── AI-QUICKSTART.md          # 5-minute quick start
└── sprints/
    ├── sprint1/                  # Completed ✅
    ├── sprint2/                  # Completed ✅
    │   └── SPRINT2-HANDOFF.md   # Handoff document
    └── sprint3/                  # CURRENT 🚀 (80% complete!)
        ├── sprint-plan.md        # Current sprint plan
        └── sprint-log.md         # Daily progress ← READ THIS!
```

### Code Structure
```
src/
├── DevMetricsPro.Core/              # Domain entities, interfaces
├── DevMetricsPro.Application/       # Business logic, DTOs, services
│   ├── DTOs/Charts/                 # Chart DTOs
│   ├── DTOs/Metrics/                # Advanced metric DTOs (NEW!)
│   ├── Interfaces/                  # Service interfaces
│   └── Services/                    # ChartDataService, LeaderboardService
├── DevMetricsPro.Infrastructure/    # Data access, repositories, EF Core
│   └── Services/                    # MetricsCalculationService
└── DevMetricsPro.Web/               # Blazor UI, API controllers
    ├── Components/
    │   ├── Pages/                   # Home (full dashboard!), Login, Register, etc.
    │   ├── Layout/                  # MainLayout, TopNav
    │   └── Shared/
    │       ├── Charts/              # LineChart, BarChart, ContributionHeatmap
    │       └── Leaderboard.razor    # Team leaderboard
    ├── Controllers/                 # API endpoints
    ├── Hubs/                        # MetricsHub (SignalR!)
    ├── Jobs/                        # Hangfire background jobs
    ├── Services/                    # SignalRService, MetricsHubService
    └── wwwroot/js/charts.js         # Chart.js JSInterop wrapper
```

### Helper Scripts
```
.ai/helpers/
├── test-auth-endpoints.ps1          # Test register + login
├── test-single-endpoint.ps1         # Test individual endpoint
├── decode-jwt-token.ps1             # Decode and validate JWT
├── kill-dev-server.ps1              # Stop dev server
└── kill-port.ps1                    # Free up specific port
```

---

## 🚀 What to Do Next (Phase 3.9 - Time Range Filters)

Based on `sprint-log.md`, the next phase is **Phase 3.9: Time Range Filters**:

1. **Create TimeRangeSelector Component**
   - Preset buttons (7d, 30d, 90d, 1y, All)
   - Custom date picker option
   - Emits `EventCallback` on change

2. **Create DashboardStateService**
   - Manages selected date range
   - Fires `OnStateChanged` event
   - Registered as Scoped service

3. **Update All Charts**
   - Subscribe to state changes
   - Reload data when range changes
   - Show loading states during refresh

4. **Integration**
   - Add global selector to dashboard header
   - All charts respond to changes

---

## ⚠️ Important Notes for AI

### Do's ✅
- ✅ **Read sprint log first** - Shows daily progress and learnings
- ✅ **Create GitHub issue** before each phase
- ✅ **Create feature branch** from master (never commit to master)
- ✅ **Provide detailed guidance** - User is learning Blazor/.NET
- ✅ **Can implement code** with user approval
- ✅ **Update sprint log** after completing work
- ✅ **Use read tools** - Read files, search codebase, understand context

### Don'ts ❌
- ❌ **Don't skip reading documentation** - Critical context!
- ❌ **Don't commit to master** - Always use feature branch
- ❌ **Don't guess** - If unsure, ask user or search codebase
- ❌ **Don't forget to update sprint log** - Continuity is key
- ❌ **Don't skip testing** - Verify changes work

---

## 🎓 Learning Mode

The user is **learning Blazor and .NET**, so:

1. **Explain concepts** - What is this? Why are we doing it?
2. **Reference documentation** - Point to `.cursor/*.mdc` files for details
3. **Discuss trade-offs** - Why this approach over alternatives?
4. **Encourage questions** - Make sure user understands before moving on
5. **Document learnings** - Add to sprint log for future reference

---

## 🔧 Development Commands

### Start Dev Server
```powershell
dotnet run --project src/DevMetricsPro.Web
```
App runs on: `https://localhost:5234`

### Test API Endpoints
```powershell
# Test both register and login
.\.ai\helpers\test-auth-endpoints.ps1

# Test single endpoint
.\.ai\helpers\test-single-endpoint.ps1

# Decode JWT token
.\.ai\helpers\decode-jwt-token.ps1 "your-token-here"
```

### Database
```powershell
# Start PostgreSQL + Redis
docker-compose up -d

# Create migration
dotnet ef migrations add MigrationName --project src/DevMetricsPro.Infrastructure --startup-project src/DevMetricsPro.Web

# Apply migration
dotnet ef database update --project src/DevMetricsPro.Infrastructure --startup-project src/DevMetricsPro.Web
```

### Build & Test
```powershell
# Build solution
dotnet build

# Run tests
dotnet test
```

---

## 📊 Current Sprint Status

**Sprint**: Sprint 3 - Charts & Real-time Dashboard  
**Progress**: ~80% Complete (Phases 3.1-3.8 done!)  
**Next**: Phase 3.9 - Time Range Filters

### ✅ Sprint 1 - Complete:
- Core entities (Developer, Repository, Commit, PR, Metric)
- ASP.NET Core Identity + JWT authentication
- Auth API endpoints (Register, Login)
- Blazor UI (Login, Register, Home pages)

### ✅ Sprint 2 - Complete:
- GitHub OAuth integration
- GitHub token storage
- Repository sync (36+ repos synced!)
- Commits sync (incremental updates)
- Pull Requests sync
- Hangfire background jobs
- Metrics calculation service
- Professional UI redesign

### 🏃 Sprint 3 - In Progress (80%!):
- ✅ Phase 3.1: Chart Library Setup (Chart.js via JSInterop)
- ✅ Phase 3.2: Commit Activity Chart (line chart with real data!)
- ✅ Phase 3.3: PR Statistics Bar Chart
- ✅ Phase 3.4: Contribution Heatmap (GitHub-style!)
- ✅ Phase 3.5: Team Leaderboard (with metric selector!)
- ✅ Phase 3.6: SignalR Hub Setup
- ✅ Phase 3.7: Client-Side SignalR (auto-refresh!)
- ✅ Phase 3.8: Advanced Metrics (PR review time, velocity!)
- ⏳ Phase 3.9: Time Range Filters (NEXT)
- ⏳ Phase 3.10: Polish & Performance

---

## 🔍 Quick Health Check

Before starting work, verify:

1. ✅ **Docker running**: `docker ps` shows PostgreSQL + Redis
2. ✅ **Database updated**: Migrations applied
3. ✅ **Solution builds**: `dotnet build` succeeds
4. ✅ **Dev server works**: Can access `https://localhost:5234`
5. ✅ **Auth works**: Can register and login
6. ✅ **Charts work**: Dashboard shows commit and PR charts
7. ✅ **SignalR works**: Dashboard refreshes on sync

---

## 💡 Tips for Success

1. **Start with sprint log** - Shows what works, what doesn't, and why
2. **Use helper scripts** - Test endpoints quickly with PowerShell scripts
3. **Check .cursor rules** - Quick reference for conventions
4. **Update as you go** - Keep sprint log current for next AI session
5. **Explain, don't just code** - User is learning, so teach concepts
6. **Test thoroughly** - Verify changes work before moving on

---

## 📞 If You Get Stuck

1. **Search codebase** - Use `codebase_search` or `grep` tools
2. **Read implementation** - Check existing code for patterns
3. **Check sprint log** - See how similar problems were solved
4. **Ask user** - They know the project history and context
5. **Reference docs** - `.cursor/*.mdc` files have detailed guidance

---

## 🎯 Success Criteria

You'll know you're on the right track when:

- ✅ You understand what sprint/phase we're on
- ✅ You know what's been completed and what's next
- ✅ You follow the workflow (issue → branch → implement → PR)
- ✅ You explain concepts as you implement
- ✅ You update sprint log with progress and learnings
- ✅ You ask for approval before pushing changes
- ✅ You maintain architectural principles (Clean Architecture, async/await, DTOs)

---

## 🚀 Ready to Start!

Once you've read the required files and understand the project status, you should:

1. **Confirm understanding**: "I've read the sprint log. We're in Sprint 3 at 80% complete. Phases 3.1-3.8 done (charts, heatmap, leaderboard, SignalR, advanced metrics). Next is Phase 3.9 (Time Range Filters)."
2. **Summarize status**: "Dashboard now has line charts, bar charts, heatmap, leaderboard, real-time updates, and advanced metrics!"
3. **Ask for direction**: "Would you like to continue with Phase 3.9 (Time Range Filters), or work on something else?"

Let's build something great! 🎉

---

**Last Updated**: December 2, 2025  
**Sprint**: Sprint 3, ~80% Complete (Phases 3.1-3.8 done!)  
**Version**: 5.0

