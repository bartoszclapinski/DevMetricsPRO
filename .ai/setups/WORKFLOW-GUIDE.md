# DevMetrics Pro - Development Workflow Guide 🔄

This document describes the **issue-driven, guided-implementation** workflow used in this project. This workflow is designed for learning while building production-quality code.

---

## 🎯 Core Workflow Principles

### 1. **Issue-Driven Development**
Every feature, phase, or significant change MUST have a GitHub issue created first.

### 2. **AI Guides, User Implements (or AI Implements with Approval)**
- The **AI provides guidance** on what needs to be implemented and how
- The **AI can implement code** directly with user approval
- The **USER reviews** implementations and approves changes
- The **USER tests** locally to verify functionality
- **Both collaborate** on architecture and design decisions

### 3. **Learning-First Approach**
- Explanations are provided for **WHY** we do things
- Concepts are explained **as we implement** them
- Questions are encouraged and answered immediately
- Documentation captures learnings

### 4. **Small, Incremental Steps**
- Each phase is broken into small steps
- Each step is testable independently
- Frequent commits with clear messages
- Regular verification and review

---

## 📋 Standard Workflow for Each Phase

### Step 1: Create GitHub Issue
**ALWAYS start with an issue!**

```markdown
Title: [SPRINT X] Phase X.X: [Phase Name]

Description:
- Brief description of what this phase accomplishes
- Key tasks to complete
- Acceptance criteria
- References to sprint plan
```

**Example:**
- Issue #115: [SPRINT 3] Phase 3.3: PR Statistics Bar Chart

### Step 2: Create Feature Branch
**ALWAYS work on a feature branch!**

```bash
git checkout master
git pull origin master
git checkout -b sprintX/phaseX.X-feature-name-#IssueNumber
```

**Naming convention:**
- `sprintX/phaseX.X-feature-name-#IssueNumber`
- Example: `sprint3/phase3.3-pr-stats-chart-#115`

### Step 3: AI Provides Implementation Guidance (or Implements)
**AI guides or implements with approval:**

```
Now let's implement [Feature].

You need to create [File] at [path]:

[AI explains what this file does and why]
[AI provides code structure/examples OR implements directly]
[AI explains the patterns and conventions to follow]

Here's what needs to be implemented:
[Detailed guidance and code examples]

Would you like me to implement this, or would you prefer to code it yourself?
```

### Step 4: User Reviews and Tests
**USER reviews and tests the implementation:**

```
[User reviews the code changes]
[User runs the application locally]
[User verifies functionality works as expected]

User: I've tested it, looks good! / There's an issue with X...
```

### Step 5: Push Changes
**Push the feature branch:**

```bash
git add .
git commit -m "feat(scope): description

Closes #IssueNumber"
git push -u origin sprintX/phaseX.X-feature-name-#IssueNumber
```

**Commit message format:**
```
<type>(<scope>): <description>

[optional body]

Closes #IssueNumber
```

**Types:**
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `test:` - Adding tests
- `refactor:` - Code refactoring
- `chore:` - Maintenance tasks

### Step 6: Create Pull Request
**Title format:**
```
[SPRINT X] Phase X.X: [Description] - Closes #IssueNumber
```

**Example:**
```
[SPRINT 3] Phase 3.3: PR Statistics Bar Chart - Closes #115
```

**PR Description:**
- Auto-filled from commits (due to conventional commits)
- Just check the checkboxes:
  - [ ] Builds successfully
  - [ ] CI checks passing
  - [ ] Tested locally

### Step 7: Merge and Continue
**After PR approval:**

```bash
# Merge via GitHub UI
# Then pull latest master
git checkout master
git pull origin master

# Ready for next phase!
```

### Step 8: Update Sprint Log
**AI updates sprint log with:**
- What was implemented
- What was learned
- Time spent
- Any challenges solved
- GitHub issue/branch references

---

## 🔄 Daily Development Cycle

### Morning (5 minutes)
1. Open sprint log file (`.ai/sprints/sprintX/sprint-log.md`)
2. Review yesterday's progress
3. Check GitHub issues for current phase
4. Plan today's focus

### During Development (2-3 hours)
1. **AI guides**: "Let's implement [Feature]..." [provides guidance or implements]
2. **User reviews**: "I'll test this..." [reviews and tests]
3. **AI documents**: "Let me update the sprint log..." [updates docs]
4. **Repeat** for each step in the phase
5. **Commit** after each working step

### End of Phase
1. **Verify**: Everything works locally
2. **Update sprint log**: Document learnings
3. **Commit and push**: Push feature branch
4. **Create PR**: Link to issue
5. **Merge**: After CI checks pass
6. **Update master**: Pull latest

---

## 📝 Documentation Standards

### Sprint Log Updates
**After each phase, add to sprint log:**

```markdown
### Day X - YYYY-MM-DD
**Phases completed**:
- [x] Phase X.Y: [Name] ✅

**What I implemented**:
- [Component/Feature 1]
- [Component/Feature 2]

**What I learned**:
- [Concept 1]: [Explanation]
- [Concept 2]: [Explanation]

**Time spent**: X hours
**Blockers**: None / [describe]

**GitHub**:
- Issue: #XX
- Branch: `sprintX/phaseX.X-feature-#XX`
- PR created and merged ✅
```

### Commit Messages
**ALWAYS use conventional commits:**

```bash
feat(charts): add PR statistics bar chart component
fix(service): resolve null reference in ChartDataService
docs(readme): update setup instructions
test(repository): add integration tests for UnitOfWork
refactor(logging): extract Serilog config to separate class
chore(deps): update EF Core to version 9.0.0
```

### Pull Request Templates
**We use a simple checkbox template:**

```markdown
## ✅ Ready to Merge?
- [ ] Builds successfully
- [ ] CI checks passing
- [ ] Tested locally
```

---

## 🎓 Learning Approach

### When Implementing
**AI does:**
- Provides detailed guidance on what to implement
- Can implement code directly with user approval
- Explains WHAT is being implemented
- Explains WHY we're doing it
- Provides context about architecture fit
- Shows patterns and conventions

**USER:**
- Reviews code changes
- Tests functionality locally
- Asks questions when unclear
- Approves changes for commit
- Learns by reviewing with expert guidance

### When Reviewing
**AI checks:**
- Code correctness and quality
- Adherence to conventions
- Potential improvements
- Learning opportunities

**AI explains:**
- What the code does
- Why it's important
- How it fits into the bigger picture
- Related concepts to explore

### Key Questions Pattern
**When USER asks "What is X?"**

AI answers:
1. **Definition**: What it is
2. **Purpose**: Why we use it
3. **Example**: How it works in practice
4. **Context**: Where it fits in our project

---

## 🚨 Important Rules

### AI Must NOT
- ❌ Skip creating GitHub issues
- ❌ Commit without user approval
- ❌ Assume user understanding without asking
- ❌ Rush through explanations
- ❌ Skip testing verification

### AI Must ALWAYS
- ✅ Create GitHub issues first
- ✅ Provide detailed explanations
- ✅ Ask for approval before committing
- ✅ Guide on conventional commit messages
- ✅ Help verify each step works
- ✅ Update sprint log after phases

### User Must ALWAYS
- ✅ Review code changes before approving
- ✅ Test locally after implementations
- ✅ Ask questions when unclear
- ✅ Approve commits before pushing
- ✅ Verify sprint log is updated

---

## 📊 Progress Tracking

### Sprint Log File
**Location**: `.ai/sprints/sprintX/sprint-log.md`

**Updated**:
- After completing each phase
- At end of each day
- When significant learnings occur

**Contains**:
- Daily progress with timestamps
- Phases completed
- Concepts learned
- Time tracking
- Blockers and solutions
- GitHub issue/branch references

### GitHub Issues
**Created for**:
- Each sprint phase
- Bug fixes
- Feature requests
- Technical debt

**Format**:
- Title: `[SPRINT X] Phase X.X: Description`
- Description with context
- Tasks list
- Acceptance criteria
- Labels: `sprint-X`, `enhancement`, etc.

### Git Branches
**Naming**: `sprintX/phaseX.X-feature-name-#issueNumber`

**Lifecycle**:
1. Created from `master`
2. Implements single phase/feature
3. Merged back to `master` via PR
4. Deleted after merge

---

## 🔧 Tools and Commands Reference

### Common EF Core Commands
```bash
# Add migration
dotnet ef migrations add MigrationName -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web

# Apply migrations
dotnet ef database update -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web

# Remove last migration
dotnet ef migrations remove -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web
```

### Common Git Commands
```bash
# Create feature branch
git checkout -b sprint3/phase3.X-feature-#IssueNumber

# Status and add changes
git status
git add .

# Commit with conventional commit
git commit -m "feat(scope): description

Closes #IssueNumber"

# Push feature branch
git push -u origin sprint3/phase3.X-feature-#IssueNumber

# Switch to master and pull latest
git checkout master
git pull origin master
```

### Testing Commands
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test project
dotnet test tests/DevMetricsPro.Core.Tests/
```

### Build Commands
```bash
# Build solution
dotnet build

# Build specific project
dotnet build src/DevMetricsPro.Web/

# Run dev server
dotnet run --project src/DevMetricsPro.Web
```

---

## 🎯 Phase Completion Checklist

Before marking a phase as complete:

- [ ] All code implemented and working
- [ ] User has tested locally
- [ ] Sprint log updated with learnings
- [ ] Changes committed with conventional commit messages
- [ ] Feature branch pushed to GitHub
- [ ] Pull request created and linked to issue
- [ ] CI checks passing
- [ ] PR merged to master
- [ ] Local master branch updated

---

## 🚀 Starting a New Sprint

1. **Read sprint plan**: `.ai/sprints/sprintX/sprint-plan.md`
2. **Review previous sprint**: Check handoff document if exists
3. **Create sprint log**: Update `.ai/sprints/sprintX/sprint-log.md`
4. **Create issues**: One for each phase
5. **Begin Phase 1**: Follow workflow above

---

## 🆘 When Stuck

### Technical Blockers
1. **Document the issue** in sprint log
2. **Ask AI for clarification** with specific details
3. **Research** if needed (documentation, Stack Overflow)
4. **Try alternative approach** if suggested
5. **Update sprint log** with solution

### Understanding Blockers
1. **Ask "What is X?"** for specific concepts
2. **Request examples** related to our code
3. **Ask "Why do we do Y?"** for context
4. **Request analogies** for complex topics
5. **Document understanding** in sprint log

---

## ✅ Success Criteria

This workflow is working well when:

- ✅ Every change has a GitHub issue
- ✅ All work happens on feature branches
- ✅ User reviews all code changes
- ✅ AI explains concepts thoroughly
- ✅ Sprint log is kept up to date
- ✅ Conventional commits are used consistently
- ✅ PRs are small and focused
- ✅ Learning is documented
- ✅ Progress is visible and trackable
- ✅ Code quality remains high

---

## 📚 Additional Resources

### Clean Architecture
- `.cursor/architecture.mdc` - Detailed architecture rules
- `.cursor/dotnet-conventions.mdc` - C# coding standards

### Blazor Development
- `.cursor/blazor-rules.mdc` - Blazor patterns and practices

### Database
- `.cursor/database-rules.mdc` - EF Core and PostgreSQL guide

### Testing
- `.cursor/testing-rules.mdc` - Testing standards

### Project Context
- `.ai/setups/prd.md` - Product requirements
- `.ai/setups/GETTING-STARTED.md` - Quick start guide
- `.ai/sprints/overall-plan.md` - 5-sprint roadmap

---

## 🎉 Remember

> **This is a learning project!** The goal is not just to build an application, but to:
> - **Understand** Clean Architecture
> - **Master** .NET 9 and Blazor
> - **Practice** professional development workflows
> - **Build** production-quality code
> - **Document** the learning journey

**Take your time. Ask questions. Understand deeply.**

---

**Last Updated**: December 4, 2025  
**Sprint**: Sprint 3 Complete ✅ (All 10 Phases!)  
**Status**: Ready for Sprint 4 Planning  
**Next**: Sprint 4 - Features TBD

