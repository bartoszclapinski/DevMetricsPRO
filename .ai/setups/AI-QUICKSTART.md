# 🚀 AI Quick Start Guide

> **TL;DR**: Get a new AI assistant up to speed in 5 minutes.

---

## 📚 Read These Files First (In Order)

1. **`.ai/sprints/sprint3/sprint-log.md`** ← Start here! (Shows what's done, what's next)
2. **`.ai/setups/WORKFLOW-GUIDE.md`** ← How we work (AI guides, user implements)
3. **`.cursor/rules.md`** ← Core principles and conventions
4. **`.ai/setups/prd.md`** ← What we're building

---

## ⚡ Current Status (As of Dec 2, 2025)

- **Sprint**: Sprint 3 - Charts & Real-time Dashboard 📊
- **Progress**: Phase 3.8 complete ✅ (~80% of Sprint 3)
- **Next Phase**: Phase 3.9 - Time Range Filters
- **Branch**: Currently on `master` (all features merged!)

### ✅ What's Working:

**Sprint 1 Complete** ✅:
- Authentication (JWT, Identity)
- Blazor UI (Login, Register, Dashboard)
- Clean Architecture foundation

**Sprint 2 Complete** ✅:
- GitHub OAuth integration
- Repository sync (36+ repos)
- Commits sync (incremental)
- Pull Requests sync
- Hangfire background jobs
- Metrics calculation service
- Professional UI redesign

**Sprint 3 In Progress** 🚀 (80% Complete!):
- ✅ Phase 3.1: Chart.js integrated via JSInterop
- ✅ Phase 3.2: Commit Activity Line Chart (real data!)
- ✅ Phase 3.3: PR Statistics Bar Chart
- ✅ Phase 3.4: Contribution Heatmap (GitHub-style!)
- ✅ Phase 3.5: Team Leaderboard
- ✅ Phase 3.6: SignalR Hub Setup
- ✅ Phase 3.7: Client-Side SignalR (real-time!)
- ✅ Phase 3.8: Advanced Metrics (PR review time, code velocity!)
- ⏳ Phase 3.9: Time Range Filters (NEXT)
- ⏳ Phase 3.10: Polish & Performance

### ⏭️ What's Next:
- Phase 3.9: Global time range filter component
- Phase 3.10: Polish, performance, responsive design

---

## 🎯 Key Rules

1. **Always create GitHub issue first** for each phase
2. **Always create feature branch** (never commit to master)
3. **AI can implement code** with user approval
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
- **Chart.js** (via JSInterop)
- **SignalR** (real-time updates!)
- Hangfire (background jobs)

---

## 📁 Project Structure

```
.ai/
├── setups/                   # Project documentation
│   ├── AI-QUICKSTART.md     ← YOU ARE HERE
│   ├── PROJECT-STRUCTURE.md ← IMPORTANT: Check before implementing!
│   └── ...
├── sprints/
│   ├── sprint1/             # Completed ✅
│   ├── sprint2/             # Completed ✅
│   └── sprint3/             # CURRENT 🚀 (80% complete!)
│       ├── sprint-plan.md   # What to do
│       └── sprint-log.md    # What's done ← READ THIS!

src/
├── DevMetricsPro.Core/       # Entities, interfaces
├── DevMetricsPro.Application/ # Services, DTOs
│   ├── DTOs/Charts/          # Chart DTOs
│   ├── DTOs/Metrics/         # Advanced metric DTOs (NEW!)
│   ├── Interfaces/           # IChartDataService, ILeaderboardService, IMetricsHubService
│   └── Services/             # ChartDataService, LeaderboardService
├── DevMetricsPro.Infrastructure/ # EF Core, repos
│   └── Services/             # MetricsCalculationService (with advanced metrics!)
└── DevMetricsPro.Web/        # Blazor + API
    ├── Components/
    │   ├── Pages/            # Home (with charts!), Login, etc.
    │   └── Shared/
    │       ├── Charts/       # LineChart, BarChart, ContributionHeatmap
    │       └── Leaderboard.razor
    ├── Hubs/                 # MetricsHub (SignalR!)
    ├── Services/             # SignalRService, MetricsHubService
    └── wwwroot/js/charts.js  # Chart.js wrapper
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

- [ ] Read `sprint3/sprint-log.md` - Know what's complete
- [ ] Read `WORKFLOW-GUIDE.md` - Understand workflow
- [ ] Confirm current branch (should be on feature branch or master)
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
2. **AI creates GitHub issue** (title: [SPRINT X] Phase X.X: Description)
3. **AI creates feature branch** (`sprintX/phaseX.X-feature-#IssueNumber`)
4. **AI implements code** (with explanations)
5. **User reviews and tests**
6. **Commit with issue reference** (`Closes #XX`)
7. **Push and create PR**
8. **Merge to master**
9. **AI updates sprint log**
10. **Repeat** for next phase

---

## 🚀 First Message to Send

After reading this, confirm you understand:

```
I've reviewed the project status:
- Sprint 1: Complete ✅ (Authentication)
- Sprint 2: Complete ✅ (GitHub integration, background jobs, metrics!)
- Sprint 3: In Progress 🚀 (~80% complete!)
  - ✅ Phases 3.1-3.8: All charts, heatmap, leaderboard, SignalR, advanced metrics
  - ⏳ Phase 3.9: Time Range Filters (NEXT)
  - ⏳ Phase 3.10: Polish & Performance

The dashboard now has:
- 📊 Commit activity line chart
- 📈 PR statistics bar chart
- 🗓️ GitHub-style contribution heatmap
- 🏆 Team leaderboard with metrics
- ⚡ Real-time updates via SignalR
- 📉 Advanced metrics (PR review time, code velocity)

Would you like to:
1. Continue with Phase 3.9 (Time Range Filters)?
2. Jump to Phase 3.10 (Polish & Performance)?
3. Work on something else?

I'm ready to implement, explain, and keep docs updated! 🎯
```

---

**Pro Tip**: When stuck, check `sprint-log.md` - it has solutions to common problems we've already solved!

---

**Last Updated**: December 2, 2025  
**Sprint**: Sprint 3 - Charts & Real-time Dashboard  
**Phase**: 3.8 Complete ✅, Next: 3.9  
**Version**: 4.0

