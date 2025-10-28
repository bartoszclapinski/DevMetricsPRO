# Sprint 2 - GitHub Integration & Data Pipeline - Log

**Start Date**: October 28, 2025  
**End Date**: TBD  
**Status**: 🏃 In Progress  

---

## 🎯 Sprint Goal
Integrate with GitHub to fetch and sync developer metrics with background processing

---

## 📊 Weekly Progress

## WEEK 1: GitHub Integration

### Day 1 - October 28, 2025
**Phases completed**:
- [x] Phase 2.1: GitHub OAuth Setup ✅

**What I learned**:

**Phase 2.1 - GitHub OAuth Setup:**
- Created comprehensive GitHub OAuth DTOs (Request, Response, Callback)
- Implemented `IGitHubOAuthService` interface in Application layer
- Implemented `GitHubOAuthService` in Infrastructure layer with:
  - `GetAuthorizationUrl()` - Generates GitHub OAuth URL with state parameter
  - `ExchangeCodeForTokenAsync()` - Exchanges authorization code for access token
  - `GetUserInfoAsync()` - Retrieves user information from GitHub API
- Learned GitHub OAuth requires **form-encoded POST**, not JSON:
  - Used `FormUrlEncodedContent` instead of `PostAsJsonAsync`
  - GitHub's token endpoint: `https://github.com/login/oauth/access_token`
- Learned GitHub API responses use **snake_case** JSON properties:
  - `access_token` → Need `[JsonPropertyName("access_token")]`
  - `token_type` → Need `[JsonPropertyName("token_type")]`
  - `avatar_url` → Need `[JsonPropertyName("avatar_url")]`
- Created `GitHubController` with three endpoints:
  - GET `/api/github/authorize` - Initiates OAuth flow
  - GET `/api/github/callback` - Handles GitHub redirect
  - GET `/api/github/test` - Simple test endpoint
- Added "Connect GitHub" button to `Home.razor`:
  - Calls `/api/github/authorize` to get auth URL
  - Navigates browser to GitHub for authorization
  - GitHub redirects back to `/api/github/callback` with code
- Configured session support for OAuth state validation:
  - Added `AddDistributedMemoryCache()` and `AddSession()` to DI
  - Used `HttpContext.Session` to store CSRF state parameter
- Registered services in DI container:
  - `AddHttpClient()` for `IHttpClientFactory`
  - `AddScoped<IGitHubOAuthService, GitHubOAuthService>()`
- Updated `.gitignore` to exclude `DO-NOT-SHARE.md` (GitHub credentials)
- Stored GitHub OAuth credentials in User Secrets:
  - `GitHub:ClientId`
  - `GitHub:ClientSecret`
  - `GitHub:RedirectUri` (http://localhost:5234/api/github/callback)
  - `GitHub:Scopes` (repo,read:user)

**Key Concepts:**
- **OAuth 2.0 Authorization Code Flow**: Three-legged authentication flow
  1. App requests authorization (redirect to GitHub)
  2. User grants permission
  3. GitHub redirects back with authorization code
  4. App exchanges code for access token
- **CSRF Protection**: State parameter prevents cross-site request forgery
- **Access Token**: Bearer token for authenticating API requests
- **Scopes**: Permissions requested (repo, read:user, etc.)
- **Form URL Encoding**: `application/x-www-form-urlencoded` content type
- **JSON Property Mapping**: `[JsonPropertyName]` attribute for C#/JSON naming mismatches

**Challenges:**
- **Issue**: User Secrets case sensitivity
  - Problem: Set `Github:` (lowercase 'h') but config expected `GitHub:`
  - Solution: Removed and re-added secrets with correct casing
- **Issue**: `IHttpClientFactory` not found
  - Problem: Missing `Microsoft.Extensions.Http` package in Infrastructure project
  - Solution: Added NuGet package to Infrastructure.csproj
- **Issue**: Service registration typo
  - Problem: Typed `IGitHubOAuthServices` (extra 's') in Program.cs
  - Solution: Corrected to `IGitHubOAuthService`
- **Issue**: Token exchange failing
  - Problem: GitHub expects form-encoded, we were sending JSON
  - Solution: Changed from `PostAsJsonAsync` to `PostAsync` with `FormUrlEncodedContent`
- **Issue**: Access token null after exchange
  - Problem: JSON property names didn't match (`AccessToken` vs `access_token`)
  - Solution: Added `[JsonPropertyName]` attributes to internal DTOs
- **Issue**: User authentication check failing in callback
  - Problem: Session state not preserved during GitHub redirect
  - Solution: Temporarily commented out user auth check for testing
- **Issue**: HTTPS certificate issues
  - Problem: App not working on HTTPS (SSL errors)
  - Solution: Switched to HTTP for local development
- **Issue**: OAuth codes can only be used once
  - Problem: Retrying with same code failed
  - Solution: Generated fresh authorization code for each test

**Testing:**
- ✅ Created GitHub OAuth App with correct redirect URL
- ✅ Stored credentials in User Secrets (case-sensitive!)
- ✅ Authorization URL generated correctly with state parameter
- ✅ Redirect to GitHub works, user can authorize app
- ✅ GitHub redirects back to callback with authorization code
- ✅ Token exchange successful (form-encoded POST)
- ✅ Access token received from GitHub
- ✅ User info retrieved from GitHub API
- ✅ Logs show: `Successfully authenticated GitHub user bartoszclapinski`
- ✅ Full end-to-end OAuth flow working!

**Technical Debt Identified**:
- Session state validation temporarily disabled (needs fix for Blazor/API context sharing)
- User authentication check temporarily disabled in callback (needs proper user context preservation)
- Tokens not yet stored in database (Phase 2.2)
- No /settings page yet (redirects to / instead)
- Need to implement proper token encryption before storing

**Time spent**: ~4 hours  
**Blockers**: None  
**Notes**: 
- GitHub OAuth integration complete and working! 🎉
- Full authorization flow tested end-to-end
- Ready to store tokens in database (Phase 2.2)
- Issue #44 ready to be closed after PR merge
- Created feature branch: `sprint2/phase2.1-github-oauth-#44`
- Comprehensive commit pushed with detailed message
- User Secrets properly configured (keep DO-NOT-SHARE.md private!)

---

### Day 2 - __________
**Phases completed**:
- [ ] Phase 2.2: Store GitHub Tokens in Database

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 3 - __________
**Phases completed**:
- [ ] Phase 2.3: GitHub Repositories Sync (start)

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 4 - __________
**Phases completed**:
- [ ] Phase 2.3: GitHub Repositories Sync (complete)

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 5 - __________
**Phases completed**:
- [ ] Phase 2.4: Commits Sync

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Week 1 total**: ~__ hours  
**Notes**: 

---

## WEEK 2: Background Jobs & Metrics

### Day 6 - __________
**Phases completed**:
- [ ] Phase 2.5: Hangfire Setup

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 7 - __________
**Phases completed**:
- [ ] Phase 2.6: Pull Requests Sync

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 8 - __________
**Phases completed**:
- [ ] Phase 2.7: Basic Metrics Calculation

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 9 - __________
**Phases completed**:
- [ ] Phase 2.8: Week 2 Wrap-up (start)

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 10 - __________
**Phases completed**:
- [ ] Phase 2.8: Week 2 Wrap-up (complete)

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Week 2 total**: ~__ hours  
**Sprint 2 total**: ~__ hours  
**Notes**: 

---

## 🎓 Learning Summary

### GitHub OAuth Concepts
- OAuth 2.0 Authorization Code Flow (three-legged auth)
- CSRF protection with state parameter
- Access tokens and refresh tokens
- OAuth scopes and permissions
- Form URL encoding vs JSON
- Bearer token authentication
- Token security and storage

### .NET & HTTP Concepts
- HttpClientFactory for managing HTTP clients
- FormUrlEncodedContent for form submissions
- JSON property name mapping with attributes
- User Secrets for local credential storage
- Session state management
- IHttpClientFactory benefits (connection pooling, resilience)

### Blazor & API Integration
- Calling API endpoints from Blazor components
- NavigationManager for redirects
- Handling OAuth redirects in Blazor Server
- Session state sharing challenges (Blazor vs API)

### GitHub API
- GitHub OAuth Apps configuration
- Redirect URLs and callback handling
- User info endpoint (`/user`)
- API authentication with Bearer tokens
- Rate limiting considerations

---

## 📈 Metrics

- **Total time spent**: ~4 hours (estimated: 20-30h for sprint)
- **Commits made**: 2 (feature implementation + cleanup)
- **Tests written**: Manual testing (E2E OAuth flow)
- **Test coverage**: TBD
- **Phases completed**: 1 / 8
- **Success criteria met**: 1 / 11

---

## ✅ Sprint Success Criteria

- [x] GitHub OAuth working ✅
- [ ] Tokens stored securely in database
- [ ] Repositories synced from GitHub
- [ ] Commits synced in background
- [ ] Pull requests synced
- [ ] Hangfire configured and running
- [ ] Basic metrics calculated
- [ ] Dashboard displays real data
- [ ] Background jobs run reliably
- [ ] >80% test coverage
- [ ] Documentation updated

---

## 🔄 Sprint Retrospective

_(To be completed at end of sprint)_

### What went well ✅
- 

### What could be improved 🔄
- 

### GitHub API learning curve
- Easy parts:
  - 
- Challenging parts:
  - 

### Action items for Sprint 3 📝
- 

### Velocity notes
- Estimated time: 20-30 hours
- Actual time: TBD
- Accuracy: TBD

---

## 📸 Screenshots

_(Add screenshots of OAuth flow, connected account, GitHub dashboard)_

---

## 🚀 Ready for Sprint 3?

- [ ] All Sprint 2 success criteria met
- [ ] No blockers remaining
- [ ] GitHub integration working end-to-end
- [ ] Background jobs reliable
- [ ] Documentation up to date

**Date completed**: TBD  
**Release tag**: v0.3-sprint2 (to be created)

