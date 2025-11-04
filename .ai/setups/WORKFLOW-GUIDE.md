# DevMetrics Pro - Development Workflow Guide 🔄

This document describes the **issue-driven, guided-implementation** workflow used in this project. This workflow is designed for learning while building production-quality code.

---

## 🎯 Core Workflow Principles

### 1. **Issue-Driven Development**
Every feature, phase, or significant change MUST have a GitHub issue created first.

### 2. **AI Implements, User Reviews**
- The **AI implements code directly** using available tools
- The **AI explains** what is being implemented and why as it works
- The **USER reviews** the implementation in their IDE
- The **USER approves or requests changes** before committing
- The **USER learns** by understanding the implemented code and concepts
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
Title: Sprint X Phase Y.Z: [Phase Name]

Description:
- Brief description of what this phase accomplishes
- Key tasks to complete
- Acceptance criteria
- References to sprint plan
```

**Example:**
- Issue #26: Sprint 1 Phase 1.4: Logging & Error Handling

### Step 2: Create Feature Branch
**ALWAYS work on a feature branch!**

```bash
git checkout master
git pull origin master
git checkout -b sprint1/phase1.X-feature-name-#IssueNumber
```

**Naming convention:**
- `sprintX/phaseY.Z-feature-name-#IssueNumber`
- Example: `sprint1/phase1.4-logging-error-handling-#26`

### Step 3: AI Implements Code
**AI implements the code directly using tools:**

```
Now let's implement [Feature].

I'll create [File] at [path]:

[AI explains what this file does and why]
[AI uses write/search_replace tools to create/modify files]
[AI explains the code as it's being written]

Here's what I've implemented:
[Summary of changes]

Please review the changes in your IDE and let me know if you approve!
```

### Step 4: User Reviews Implementation
**USER reviews the code in their IDE:**

```
[User reviews files in IDE]
[User reads the explanations]
[User asks questions if unclear]

User: Looks good! / Can you change X?
```

### Step 5: AI Responds to Feedback
**AI addresses any feedback:**

```
[If changes requested, AI makes adjustments]
[AI explains the changes]

Ready to commit? Or would you like any other changes?
```

### Step 6: Push Changes
**AI or User pushes the feature branch:**

```bash
git push origin sprint1/phase1.4-logging-error-handling-#26
```

**Commit message format:**
```
<type>(<scope>): <description>

[optional body]

[optional footer with issue reference]

Closes #IssueNumber
```

**Types:**
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `test:` - Adding tests
- `refactor:` - Code refactoring
- `chore:` - Maintenance tasks

### Step 7: Create Pull Request
**Title format:**
```
sprintX/phaseY.Z: [Description] - Closes #IssueNumber
```

**Example:**
```
sprint1/phase1.4: Add Serilog and global exception handler - Closes #26
```

**PR Description:**
- Auto-filled from commits (due to conventional commits)
- Just check the checkboxes:
  - [ ] Builds successfully
  - [ ] CI checks passing
  - [ ] Tested locally

### Step 8: Merge and Continue
**After PR approval:**

```bash
# Merge via GitHub UI
# Then pull latest master
git checkout master
git pull origin master

# Ready for next phase!
```

---

## 🔄 Daily Development Cycle

### Morning (5 minutes)
1. Open sprint log file (`.ai/sprints/sprintX/sprint-log.md`)
2. Review yesterday's progress
3. Check GitHub issues for current phase
4. Plan today's focus

### During Development (2-3 hours)
1. **AI implements**: "Let's implement [Feature]..." [uses tools]
2. **User reviews**: "Ok, looks good..." [reviews in IDE]
3. **AI explains**: "Here's what this does and why..."
4. **Repeat** for each step in the phase
5. **AI commits** after each working step (with user approval)

### End of Phase
1. **Run tests**: Verify everything works
2. **Update sprint log**: Document learnings
3. **Commit and push**: Push feature branch
4. **Create PR**: Link to issue
5. **Merge**: After CI checks pass

### Evening (5 minutes)
1. Update sprint log with progress
2. Note time spent
3. Document any challenges
4. Plan tomorrow's focus

---

## 📝 Documentation Standards

### Sprint Log Updates
**After each phase, add to sprint log:**

```markdown
### Day X - YYYY-MM-DD
**Phases completed**:
- [x] Phase X.Y: [Name] ✅

**What I learned**:
- [Concept 1]: [Explanation]
- [Concept 2]: [Explanation]
- [Technical detail]: [Why it's important]

**Time spent**: X hours
**Blockers**: None / [describe]
**Notes**: [Additional observations]
```

### Commit Messages
**ALWAYS use conventional commits:**

```bash
feat(auth): add JWT token generation service
fix(db): resolve migration conflict in Developers table
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
- Implements code directly using tools
- Explains WHAT is being implemented
- Explains WHY we're doing it
- Provides context about architecture fit
- Shows expected outcomes

**USER:**
- Reviews changes in IDE
- Asks questions when unclear
- Approves or requests changes
- Tests implementation locally
- Learns by understanding the code

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

**Example:**
> User: "What is CancellationToken?"
> 
> AI: "CancellationToken is a mechanism to cancel long-running operations...
> [Detailed explanation with examples from our code]"

---

## 🚨 Important Rules

### AI Must NOT
- ❌ Skip creating GitHub issues
- ❌ Commit without user awareness
- ❌ Assume user understanding without asking
- ❌ Rush through explanations
- ❌ Skip testing steps
- ❌ Implement without explaining WHY

### AI Must ALWAYS
- ✅ Create GitHub issues first
- ✅ Implement code using available tools
- ✅ Explain what is being implemented and why
- ✅ Wait for user approval before committing
- ✅ Use conventional commit messages
- ✅ Verify each step works
- ✅ Update sprint log with user

### User Must ALWAYS
- ✅ Review AI-implemented changes in IDE
- ✅ Ask questions when unclear
- ✅ Approve changes before committing
- ✅ Test locally after implementation
- ✅ Create PRs for all changes
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
- Notes and observations

### GitHub Issues
**Created for**:
- Each sprint phase
- Bug fixes
- Feature requests
- Technical debt

**Format**:
- Clear title with sprint/phase reference
- Description with context
- Tasks list
- Acceptance criteria
- Labels: `sprint-X`, `phase-Y.Z`, `enhancement`, etc.

### Git Branches
**Naming**: `sprintX/phaseY.Z-feature-name-#issueNumber`

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

# Drop database (development only!)
dotnet ef database drop --force -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web
```

### Common Git Commands
```bash
# Create feature branch
git checkout -b sprint1/phase1.X-feature-#IssueNumber

# Status and add changes
git status
git add .

# Commit with conventional commit
git commit -m "feat(scope): description - Closes #IssueNumber"

# Push feature branch
git push -u origin sprint1/phase1.X-feature-#IssueNumber

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

# Clean build
dotnet clean
dotnet build
```

---

## 🎯 Phase Completion Checklist

Before marking a phase as complete:

- [ ] All code implemented according to guidance
- [ ] All tests passing locally
- [ ] Sprint log updated with learnings
- [ ] Changes committed with conventional commit messages
- [ ] Feature branch pushed to GitHub
- [ ] Pull request created and linked to issue
- [ ] CI checks passing
- [ ] Code reviewed (by AI)
- [ ] PR merged to master
- [ ] Local master branch updated
- [ ] Feature branch deleted (optional)

---

## 🚀 Starting a New Sprint

1. **Read sprint plan**: `.ai/sprints/sprintX/sprint-plan.md`
2. **Create sprint log**: `.ai/sprints/sprintX/sprint-log.md`
3. **Review overall plan**: `.ai/sprints/overall-plan.md`
4. **Create issues**: One for each phase
5. **Begin Phase 1.1**: Follow workflow above

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

### Process Questions
1. **Re-read this workflow guide**
2. **Check sprint plan** for specific phase instructions
3. **Review previous phases** for patterns
4. **Ask AI** for process clarification
5. **Update workflow guide** if needed

---

## ✅ Success Criteria

This workflow is working well when:

- ✅ Every change has a GitHub issue
- ✅ All work happens on feature branches
- ✅ User implements all code (not AI)
- ✅ Explanations are clear and helpful
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
- `.ai/prd.md` - Product requirements
- `.ai/GETTING-STARTED.md` - Quick start guide
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

**Last Updated**: November 4, 2025  
**Sprint**: Sprint 2 - Phases 2.1-2.4 Complete + UI Redesign Complete ✅  
**Next Phase**: Phase 2.5 - Hangfire Background Jobs

**Note**: This workflow reflects the **actual practice** where **AI implements code directly** using available tools while **USER reviews and approves** in their IDE. This approach combines efficiency with learning - AI implements patterns consistently while explaining concepts, and user learns by understanding and reviewing high-quality code.

