# 🚀 AI Quick Start Guide

> **TL;DR**: Get a new AI assistant up to speed in 5 minutes.

---

## 📚 Read These Files First (In Order)

1. **`.ai/sprints/sprint1/sprint-log.md`** ← Start here! (Shows what's done, what's next)
2. **`.ai/WORKFLOW-GUIDE.md`** ← How we work (AI implements, user reviews)
3. **`.cursor/rules.md`** ← Core principles and conventions
4. **`.ai/prd.md`** ← What we're building

---

## ⚡ Current Status (As of Oct 22, 2025)

- **Sprint**: Sprint 1 - Authentication & Basic UI
- **Progress**: 90% complete (Phase 1.9 done)
- **Next Phase**: Phase 1.10 - Sprint Wrap-up
- **Branch**: Currently on `docs/update-workflow-documentation`

### ✅ What's Working:
- ASP.NET Core Identity + JWT authentication
- Auth API endpoints (`/api/auth/register`, `/api/auth/login`)
- Blazor UI (Login, Register, Home pages)
- `AuthStateService` manages JWT in localStorage
- MudBlazor layout with MainLayout + NavMenu

### ⏭️ What's Next:
- Phase 1.10: Sprint wrap-up (test E2E, cleanup, docs)

---

## 🎯 Key Rules

1. **Always create GitHub issue first** for each phase
2. **Always create feature branch** (never commit to master)
3. **AI implements, user reviews** (use tools directly)
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
4. **AI implements code** (using tools, explaining concepts)
5. **User reviews in IDE** (asks questions, suggests changes)
6. **AI updates sprint log** (what was done, learnings, time spent)
7. **User approves changes**
8. **AI commits and pushes** to feature branch
9. **User creates PR and merges** on GitHub
10. **Repeat** for next phase

---

## 🚀 First Message to Send

After reading this, confirm you understand:

```
I've reviewed the project status:
- Sprint 1, Phase 1.9 complete (Authentication working!)
- Next: Phase 1.10 (Sprint Wrap-up)
- Current branch: docs/update-workflow-documentation

Would you like to:
1. Continue with Phase 1.10 (wrap up Sprint 1)
2. Work on something else?

I'm ready to implement, explain, and keep docs updated! 🎯
```

---

**Pro Tip**: When stuck, check `sprint-log.md` - it has solutions to common problems we've already solved!

---

**Last Updated**: October 22, 2025  
**Version**: 1.0

