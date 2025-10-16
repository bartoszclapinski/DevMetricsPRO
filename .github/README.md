# GitHub Actions & CI/CD Workflows

> **Note:** This is the README for GitHub Actions workflows only.  
> For the main project README, see: [../README.md](../README.md)

This folder contains automated workflows for continuous integration and deployment.

## 🚀 Workflows

### 1. `.NET CI` (`dotnet-ci.yml`)
**Runs on:** Every push and PR to master/main

**What it does:**
- ✅ **Builds** your .NET 9 solution
- ✅ **Runs tests** with code coverage
- ✅ **Spins up services** (PostgreSQL & Redis) for integration tests
- ✅ **Checks code quality** with Roslyn analyzers
- ✅ **Scans for security issues** with DevSkim
- ✅ **Verifies formatting** with dotnet format
- ✅ **Reviews dependencies** for vulnerabilities

**Status Badge:**
```markdown
![.NET CI](https://github.com/bartoszclapinski/DevMetricsPRO/workflows/.NET%20CI/badge.svg)
```

---

### 2. `PR Checks` (`pr-checks.yml`)
**Runs on:** Every PR (opened, updated)

**What it does:**
- ✅ **Validates PR title** follows conventional commits format
  - `feat:`, `fix:`, `docs:`, `refactor:`, etc.
- ✅ **Labels PR by size** (XS, S, M, L, XL)
- ✅ **Warns on large PRs** (>1000 lines)
- ✅ **Checks for merge conflicts**
- ✅ **Auto-labels** based on changed files

**PR Title Examples:**
```
✅ feat: add developer dashboard component
✅ fix: resolve null reference in repository service
✅ docs: update sprint 0 documentation
✅ refactor: improve entity configurations
❌ Added new feature (missing type prefix)
```

---

## 📊 What Gets Checked

| Check | Description | Blocks Merge? |
|-------|-------------|---------------|
| **Build** | Code compiles successfully | ✅ Yes |
| **Tests** | All tests pass | ✅ Yes |
| **Code Style** | Follows .NET conventions | ⚠️ Warning |
| **Security** | No known vulnerabilities | ⚠️ Warning |
| **Format** | Code is properly formatted | ⚠️ Warning |
| **Dependencies** | No vulnerable packages | ⚠️ Warning |
| **PR Title** | Follows format | ⚠️ Warning |

---

## 🔧 Local Testing

Before pushing, test locally:

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build --configuration Release

# Run tests
dotnet test

# Check formatting
dotnet format --verify-no-changes

# Check code style
dotnet build /p:EnforceCodeStyleInBuild=true
```

---

## 🎯 Required Checks for Merge

For a PR to be mergeable:
1. ✅ Build must pass
2. ✅ All tests must pass
3. ✅ No merge conflicts
4. ⚠️ Other checks should pass (but won't block)

---

## 📝 Adding Status Badges

Add to your main `README.md`:

```markdown
## Build Status

![.NET CI](https://github.com/bartoszclapinski/DevMetricsPRO/workflows/.NET%20CI/badge.svg)
![PR Checks](https://github.com/bartoszclapinski/DevMetricsPRO/workflows/PR%20Checks/badge.svg)
```

---

## 🏷️ Automatic Labels

PRs are automatically labeled based on what files changed:

| Label | Triggered by |
|-------|-------------|
| `documentation` | Changes to `.md` or `.ai/` files |
| `infrastructure` | Changes to Infrastructure layer |
| `core` | Changes to Core layer |
| `application` | Changes to Application layer |
| `web` | Changes to Web layer or `.razor` files |
| `tests` | Changes to test files |
| `database` | Changes to migrations or data files |
| `dependencies` | Changes to `.csproj` files |
| `size/XS` to `size/XL` | Based on lines changed |

---

## ⚙️ Configuration

### Modify Build Steps
Edit `.github/workflows/dotnet-ci.yml`

### Change PR Title Rules
Edit `.github/workflows/pr-checks.yml`:
```yaml
types: |
  feat    # New feature
  fix     # Bug fix
  docs    # Documentation
  style   # Code style
  refactor # Code refactoring
  test    # Tests
  chore   # Maintenance
  perf    # Performance
```

### Adjust Auto-labeling
Edit `.github/labeler.yml`

---

## 🔍 Viewing Results

1. Go to your repo on GitHub
2. Click **"Actions"** tab
3. See all workflow runs
4. Click on a run to see details
5. Check logs if something fails

---

## 💡 Best Practices

1. **Green CI before merge** - Always ensure CI passes
2. **Small PRs** - Keep under 500 lines for easier review
3. **Descriptive PR titles** - Follow conventional commits
4. **Fix warnings** - Even if they don't block merge
5. **Run tests locally** - Catch issues before pushing
6. **Review failed checks** - Click on failed checks to see why

---

## 🆘 Troubleshooting

### Build Fails
- Check error in workflow logs
- Try building locally: `dotnet build`
- Ensure all dependencies are restored

### Tests Fail
- Run tests locally: `dotnet test`
- Check test output for details
- Ensure database/Redis are running (for integration tests)

### Format Check Fails
- Run: `dotnet format`
- Commit formatting changes
- Push again

### Security Scan Finds Issues
- Review DevSkim results
- Fix security issues
- Consider if false positive (can be suppressed)

---

## 📚 Learn More

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)

---

**Questions?** Check workflow logs or open an issue!

