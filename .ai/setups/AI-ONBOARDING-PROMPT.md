# 🤖 AI Assistant Onboarding Prompt for DevMetrics Pro

> **Purpose**: This document provides a complete onboarding prompt for a new AI assistant to seamlessly continue development on DevMetrics Pro.

---

## 📋 Initial Prompt for New AI Session

Copy and use this prompt to start a new AI session:

```
Hi! I need your help continuing development on DevMetrics Pro, a real-time developer analytics dashboard built with .NET 9 and Blazor Server.

Before we start, please read these files IN THIS ORDER to understand the project:

1. **Project Overview & Tech Stack**:
   - Read `.ai/prd.md` - Product Requirements Document
   - Read `README.md` - Quick project overview

2. **Current Sprint Status**:
   - Read `.ai/sprints/sprint1/sprint-plan.md` - Current sprint plan
   - Read `.ai/sprints/sprint1/sprint-log.md` - What's been done so far
   - Check the "Sprint Success Criteria" section to see completed phases

3. **Development Workflow**:
   - Read `.ai/WORKFLOW-GUIDE.md` - HOW we work together
   - Read `.ai/DOCUMENTATION-STATUS.md` - Documentation status

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
- ✅ Know we're in **Sprint 1** (Authentication & Basic UI)
- ✅ Know **Phase 1.9** is complete (Login/Register UI working)
- ✅ Know the **next phase** is **Phase 1.10** (Sprint Wrap-up)
- ✅ Understand **Phases 1.4 and 1.5 were skipped** (Logging and role seeding moved to later)

### 2. **Understand the Workflow**
- ✅ **AI implements code directly** using tools (not user)
- ✅ **User reviews and approves** changes in their IDE
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
├── prd.md                           # Product requirements
├── WORKFLOW-GUIDE.md                # HOW we work
├── DOCUMENTATION-STATUS.md          # Current status
└── sprints/
    └── sprint1/
        ├── sprint-plan.md           # Current sprint plan
        └── sprint-log.md            # Daily progress log
```

### Code Structure
```
src/
├── DevMetricsPro.Core/              # Domain entities, interfaces
├── DevMetricsPro.Application/       # Business logic, DTOs, services
├── DevMetricsPro.Infrastructure/    # Data access, repositories, EF Core
└── DevMetricsPro.Web/               # Blazor UI, API controllers, SignalR hubs
    ├── Components/
    │   ├── Pages/                   # Blazor pages (Login, Register, Home)
    │   └── Layout/                  # MainLayout, NavMenu
    ├── Controllers/                 # API endpoints (AuthController)
    └── Services/                    # Client-side services (AuthStateService)
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

## 🚀 What to Do Next (Phase 1.10 - Sprint Wrap-up)

Based on `sprint-log.md`, the next phase is **Phase 1.10: Sprint Wrap-up**:

1. **Test end-to-end authentication flow**
   - Register new user
   - Login with credentials
   - Navigate between pages while authenticated
   - Logout and verify redirect

2. **Code cleanup**
   - Remove debug logs (if any remain)
   - Ensure all code follows conventions
   - Check for any TODO comments

3. **Documentation updates**
   - Update sprint log with final summary
   - Document any technical debt
   - Note lessons learned

4. **Prepare for Sprint 2**
   - Review what's complete
   - Identify blockers or issues
   - Plan next sprint focus

---

## ⚠️ Important Notes for AI

### Do's ✅
- ✅ **Read sprint log first** - Shows daily progress and learnings
- ✅ **Create GitHub issue** before each phase
- ✅ **Create feature branch** from master (never commit to master)
- ✅ **Explain what you're doing** - User is learning Blazor/.NET
- ✅ **Update sprint log** after completing work
- ✅ **Ask for approval** before pushing changes
- ✅ **Use tools directly** - Read files, edit code, run commands

### Don'ts ❌
- ❌ **Don't skip reading documentation** - Critical context!
- ❌ **Don't commit to master** - Always use feature branch
- ❌ **Don't guess** - If unsure, ask user or search codebase
- ❌ **Don't implement without explaining** - User is learning
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

**Sprint**: Sprint 1 - Authentication & Basic UI  
**Progress**: 90% Complete (Phase 1.9 done)  
**Next**: Phase 1.10 - Sprint Wrap-up

### ✅ Completed Phases:
- Phase 1.1: Core entities (Developer, Repository, Commit, PR, Metric)
- Phase 1.2: ASP.NET Core Identity integration
- Phase 1.3: JWT authentication service
- Phase 1.6: Seed script for default roles/admin user
- Phase 1.7: Auth API endpoints (Register, Login)
- Phase 1.8: Blazor AuthStateService (localStorage management)
- Phase 1.9: Blazor UI (Login, Register, Home pages, MainLayout update)

### ⏭️ Skipped (Moved to Later):
- Phase 1.4: Structured logging (Serilog) - Will do in Sprint 2+
- Phase 1.5: Role seeding in AuthController - Temporary workaround in place

### 🎯 Next Up:
- Phase 1.10: Sprint Wrap-up (testing, cleanup, documentation)

---

## 🔍 Quick Health Check

Before starting work, verify:

1. ✅ **Docker running**: `docker ps` shows PostgreSQL + Redis
2. ✅ **Database updated**: Migrations applied
3. ✅ **Solution builds**: `dotnet build` succeeds
4. ✅ **Dev server works**: Can access `https://localhost:5234`
5. ✅ **Auth works**: Can register and login

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

1. **Confirm understanding**: "I've read the sprint log. We're on Phase 1.9 (complete), next is Phase 1.10 (Sprint Wrap-up)."
2. **Summarize status**: "Authentication API and UI are working. Login, Register, and Home pages functional."
3. **Ask for direction**: "Would you like to continue with Phase 1.10 (Sprint Wrap-up), or work on something else?"

Let's build something great! 🎉

---

**Last Updated**: October 22, 2025  
**Sprint**: Sprint 1, Phase 1.9 Complete  
**Version**: 1.0

