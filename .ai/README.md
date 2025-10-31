# 🤖 AI Assistant Onboarding Guide

Welcome! This directory contains everything you need to get up to speed on the DevMetrics Pro project.

---

## 📍 **START HERE** → Quick Start

### For Brand New AI Assistants:
👉 **Read this first**: [`.ai/setups/AI-QUICKSTART.md`](.ai/setups/AI-QUICKSTART.md)

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
│   │   ├── sprint-plan.md
│   │   └── sprint-log.md       ← What was done
│   └── sprint2/                ← Current Sprint
│       ├── sprint-plan.md
│       └── sprint-log.md       ← Current progress
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

4. **Current Sprint Log** (5 min)
   - `sprints/sprint2/sprint-log.md` (if exists)
   - Shows what's been done in current sprint

5. **[`.cursor/rules.md`](../.cursor/rules.md)** (10 min)
   - Core principles
   - Naming conventions
   - Architecture rules

### 2️⃣ Context (Read if needed)

5. **[`setups/prd.md`](setups/prd.md)** (15 min)
   - Full product requirements
   - Features, tech stack, architecture

6. **[`sprints/overall-plan.md`](sprints/overall-plan.md)** (10 min)
   - Complete project timeline
   - All 5 sprints overview

7. **Current Sprint Plan** (10 min)
   - `sprints/sprint2/sprint-plan.md`
   - Detailed phase-by-phase instructions

### 3️⃣ Deep Dive (Advanced)

8. **[`setups/AI-ONBOARDING-PROMPT.md`](setups/AI-ONBOARDING-PROMPT.md)** (20 min)
   - Complete onboarding guide
   - Detailed explanations

9. **Architecture Rules** (as needed)
   - `.cursor/architecture.mdc`
   - `.cursor/dotnet-conventions.mdc`
   - `.cursor/blazor-rules.mdc`
   - `.cursor/database-rules.mdc`
   - `.cursor/testing-rules.mdc`

---

## ⚡ Super Quick Context (30 seconds)

**Project**: Real-time developer analytics dashboard  
**Tech**: .NET 9, Blazor Server, PostgreSQL, Redis, MudBlazor  
**Architecture**: Clean Architecture (Core → Application → Infrastructure → Web)  
**Current Sprint**: Sprint 2 - GitHub Integration  
**Current Phase**: Phase 2.1 complete (GitHub OAuth working) ✅  
**Next**: Phase 2.2 - Store tokens in database  

**Your Role**: Guide the user through implementation, explain concepts, update documentation

---

## 🎯 What You Should Know

### The User:
- ✅ Experienced developer
- 📚 Learning Blazor and .NET 9
- 🔧 Implements all code themselves
- 💬 Wants explanations and guidance

### The Workflow:
1. User asks what's next
2. You guide them through implementation
3. You explain concepts as you go
4. User implements in their IDE
5. You update documentation
6. Repeat!

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
- `sprints/sprint2/sprint-log.md` - Track daily progress
- `sprints/sprint2/sprint-plan.md` - What needs to be done

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

### 🏃 In Progress (Sprint 2 - Phase 2.1):
- ✅ GitHub OAuth DTOs
- ✅ GitHub OAuth Service
- ✅ GitHub Controller endpoints
- ✅ Connect GitHub button in UI
- ✅ OAuth flow tested successfully

### ⏭️ Up Next (Sprint 2 - Phase 2.2+):
- Store GitHub tokens in database
- Link GitHub account to user
- Fetch repositories from GitHub API
- Sync commits in background
- Calculate metrics

---

## 💡 Pro Tips

1. **When you start**: Read the sprint log first to see recent progress
2. **Before implementing**: Check `.cursor/rules.md` for conventions
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
→ Read current [`sprint-log.md`](sprints/sprint2/sprint-log.md)

### Understanding what to do next?
→ Read current [`sprint-plan.md`](sprints/sprint2/sprint-plan.md)

---

## 🎬 Ready to Start?

Great! Here's your first message to the user after reading the docs:

```
I've reviewed the project status:
- Sprint 1: Complete ✅ (Authentication working!)
- Sprint 2: Phase 2.1 complete ✅ (GitHub OAuth working!)
- Next: Phase 2.2 (Store tokens in database)

I see we just successfully implemented GitHub OAuth integration. 
The auth flow is working end-to-end! 🎉

Would you like to:
1. Continue with Phase 2.2 (store tokens in database)?
2. Create a PR to close issue #44 first?
3. Work on something else?

I'm ready to guide you through implementation and explain concepts! 🚀
```

---

**Last Updated**: October 28, 2025  
**Current Sprint**: Sprint 2 - GitHub Integration  
**Current Phase**: Phase 2.1 Complete ✅  
**Version**: 2.0

