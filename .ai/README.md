# 🤖 AI Assistant Onboarding Guide

Welcome! This directory contains everything you need to get up to speed on the DevMetrics Pro project.

---

## 📍 **START HERE** → Quick Start

### For Brand New AI Assistants:
👉 **Read this first**: [`.ai/setups/AI-QUICKSTART.md`](setups/AI-QUICKSTART.md)

This 5-minute guide tells you:
- What's been completed
- What's next
- Where to find everything
- How to work with the user

---

## 📁 Directory Structure

```
.ai/
├── README.md                    ← YOU ARE HERE
├── setups/                      ← Project documentation
│   ├── AI-QUICKSTART.md        ← **START HERE** (5 min read)
│   ├── PROJECT-STRUCTURE.md    ← **IMPORTANT** Complete codebase map
│   ├── WORKFLOW-GUIDE.md       ← How we work together
│   ├── prd.md                  ← Product requirements
│   ├── project-idea.md         ← Original project concept
│   └── ...                     ← More detailed docs
├── sprints/                     ← Sprint plans & logs
│   ├── overall-plan.md         ← Complete project timeline
│   ├── sprint1/                ← Completed! ✅
│   ├── sprint2/                ← Completed! ✅
│   │   └── SPRINT2-HANDOFF.md  ← Handoff document
│   ├── sprint3/                ← **COMPLETED!** ✅
│   │   ├── sprint-plan.md
│   │   └── sprint-log.md       ← Complete progress log
│   └── sprint4/                ← **NEXT SPRINT** 🚀
├── helpers/                     ← Useful scripts
│   ├── cursor-shortcuts.md     ← IDE shortcuts
│   ├── test-auth-endpoints.ps1 ← Test scripts
│   └── ...                     ← More tools
└── design/                      ← UI prototypes
    └── design-prototype.html   ← Dashboard mockup
```

---

## 🚀 Recommended Reading Order

### 1️⃣ Essentials (Must Read)
Read these to understand what you're working on:

1. **[`setups/AI-QUICKSTART.md`](setups/AI-QUICKSTART.md)** (5 min)
   - Current status, what's done, what's next

2. **[`setups/PROJECT-STRUCTURE.md`](setups/PROJECT-STRUCTURE.md)** (10 min) ⚠️ **CRITICAL**
   - Complete project structure map
   - All existing DTOs, services, entities, pages
   - Check BEFORE implementing ANYTHING to avoid duplication

3. **[`setups/WORKFLOW-GUIDE.md`](setups/WORKFLOW-GUIDE.md)** (10 min)
   - How user and AI collaborate
   - Git workflow
   - Issue creation process

4. **Latest Sprint Log** (5 min)
   - `sprints/sprint3/sprint-log.md` (completed sprint)
   - Shows everything built in Sprint 3

5. **[`.cursor/rules.md`](../.cursor/rules.md)** (10 min)
   - Core principles
   - Naming conventions
   - Architecture rules

### 2️⃣ Context (Read if needed)

6. **[`setups/prd.md`](setups/prd.md)** (15 min)
   - Full product requirements
   - Features, tech stack, architecture

7. **[`sprints/overall-plan.md`](sprints/overall-plan.md)** (10 min)
   - Complete project timeline
   - All 5 sprints overview

8. **Sprint 4 Plan** (when available)
   - `sprints/sprint4/sprint-plan.md`
   - Next phase instructions

### 3️⃣ Deep Dive (Advanced)

9. **[`setups/AI-ONBOARDING-PROMPT.md`](setups/AI-ONBOARDING-PROMPT.md)** (20 min)
   - Complete onboarding guide
   - Detailed explanations

10. **Architecture Rules** (as needed)
   - `.cursor/architecture.mdc`
   - `.cursor/dotnet-conventions.mdc`
   - `.cursor/blazor-rules.mdc`
   - `.cursor/database-rules.mdc`
   - `.cursor/testing-rules.mdc`

---

## ⚡ Super Quick Context (30 seconds)

**Project**: Real-time developer analytics dashboard  
**Tech**: .NET 9, Blazor Server, PostgreSQL, Redis, MudBlazor, Chart.js, SignalR  
**Architecture**: Clean Architecture (Core → Application → Infrastructure → Web)  
**Current Status**: Sprint 3 COMPLETE! ✅ Sprint 4 Planning Phase  
**Last Completed**: Phase 3.10 - Polish & Performance  

**Your Role**: Guide the user through implementation, explain concepts, update documentation

---

## 🎯 What You Should Know

### The User:
- ✅ Experienced developer
- 📚 Learning Blazor and .NET 9
- 🔧 Implements all code themselves (or AI implements with approval)
- 💬 Wants explanations and guidance

### The Workflow:
1. User asks what's next
2. Create GitHub issue for the phase
3. Create feature branch
4. AI guides / implements with user approval
5. User reviews and tests
6. Commit, push, create PR, merge
7. Update documentation
8. Repeat!

### Key Rules:
- ✅ **Always** explain WHY, not just WHAT
- ✅ **Always** update sprint logs after work
- ✅ **Always** create GitHub issues before phases
- ✅ **Always** use feature branches
- ❌ **Never** commit without user approval
- ❌ **Never** skip documentation updates

---

## 📝 Most Important Files

### Right Now (Current Work):
- `sprints/sprint3/sprint-log.md` - Completed Sprint 3 summary
- `sprints/sprint4/` - Next sprint (planning phase)

### Frequently Referenced:
- **`setups/PROJECT-STRUCTURE.md`** - ⚠️ **CHECK FIRST** before implementing!
- `.cursor/rules.md` - Core conventions
- `setups/WORKFLOW-GUIDE.md` - How we work
- `setups/prd.md` - What we're building

### Helper Tools:
- `helpers/cursor-shortcuts.md` - IDE productivity
- `helpers/test-auth-endpoints.ps1` - Test APIs

---

## 🔧 Quick Commands

```powershell
# Start dev server
dotnet run --project src/DevMetricsPro.Web

# Start Docker (PostgreSQL + Redis)
docker-compose up -d

# Build solution
dotnet build

# Run tests
dotnet test

# Test auth endpoints
.\.ai\helpers\test-auth-endpoints.ps1
```

---

## 🎓 Learning Mode Active

The user is learning, so:
- ✅ Explain concepts clearly
- ✅ Reference documentation
- ✅ Encourage questions
- ✅ Document learnings in sprint log

---

## 📊 Current Status Overview

### ✅ Completed (Sprint 1):
- Core domain entities (5 entities)
- Database setup (PostgreSQL + EF Core)
- Repository pattern + Unit of Work
- ASP.NET Core Identity + JWT
- Auth API endpoints (register/login)
- Basic Blazor UI with MudBlazor
- Complete authentication flow

### ✅ Completed (Sprint 2):
- GitHub OAuth integration
- GitHub token storage
- Repository sync (36+ repos)
- Commits sync with incremental updates
- Pull Requests sync
- Hangfire background jobs
- Metrics calculation service
- Professional UI redesign
- All data displaying on dashboard

### ✅ Completed (Sprint 3) - 100% Done! 🎉
- ✅ Phase 3.1: Chart Library Setup (Chart.js)
- ✅ Phase 3.2: Commit Activity Chart (real data)
- ✅ Phase 3.3: PR Statistics Bar Chart
- ✅ Phase 3.4: Contribution Heatmap (GitHub-style!)
- ✅ Phase 3.5: Team Leaderboard (sortable!)
- ✅ Phase 3.6: SignalR Hub Setup
- ✅ Phase 3.7: Client-Side SignalR (real-time!)
- ✅ Phase 3.8: Advanced Metrics (PR review, velocity!)
- ✅ Phase 3.9: Time Range Filters (global filter!)
- ✅ Phase 3.10: Polish & Performance (skeleton loaders, accessibility!)

### 🚀 Next (Sprint 4):
- Planning phase - to be defined

---

## 💡 Pro Tips

1. **When you start**: Read the sprint log first to see recent progress
2. **Before implementing**: Check `PROJECT-STRUCTURE.md` for existing code
3. **After completing work**: Update the sprint log immediately
4. **When stuck**: Check past sprint logs for solutions we've already solved
5. **Before pushing**: Always ask for user approval

---

## 🆘 Need Help?

### Understanding the workflow?
→ Read [`setups/WORKFLOW-GUIDE.md`](setups/WORKFLOW-GUIDE.md)

### Understanding the architecture?
→ Read [`.cursor/architecture.mdc`](../.cursor/architecture.mdc)

### Understanding current status?
→ Read [`sprint3/sprint-log.md`](sprints/sprint3/sprint-log.md)

### Understanding what's been built?
→ Read [`setups/PROJECT-STRUCTURE.md`](setups/PROJECT-STRUCTURE.md)

---

## 🎬 Ready to Start?

Great! Here's your first message to the user after reading the docs:

```
I've reviewed the project status:
- Sprint 1: Complete ✅ (Authentication)
- Sprint 2: Complete ✅ (GitHub integration, background jobs, metrics!)
- Sprint 3: Complete ✅ (Charts, real-time dashboard, all 10 phases done!)

The dashboard now features:
- 📊 Commit activity line chart
- 📈 PR statistics bar chart
- 🗓️ GitHub-style contribution heatmap
- 🏆 Team leaderboard with sortable metrics
- ⚡ Real-time updates via SignalR
- 📉 Advanced metrics (PR review time, code velocity)
- 🕐 Global time range filter
- ✨ Skeleton loading states & accessibility

Would you like to:
1. Plan Sprint 4?
2. Review/test the current features?
3. Work on something specific?

I'm ready to help! 🚀
```

---

**Last Updated**: December 4, 2025  
**Current Sprint**: Sprint 3 Complete! ✅ Ready for Sprint 4  
**Dashboard Status**: Fully functional with charts, real-time updates, and polish!  
**Version**: 4.0
