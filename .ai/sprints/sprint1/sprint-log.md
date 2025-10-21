# Sprint 1 - Foundation & Architecture - Log

**Start Date**: October 17, 2025  
**End Date**: TBD  
**Status**: 🏃 In Progress  

---

## 🎯 Sprint Goal
Solid foundation with domain entities, database, authentication, and basic UI

---

## 📊 Weekly Progress

## WEEK 1: Core Setup & Data Layer

### Day 1 - October 17, 2025
**Phases completed**:
- [x] Phase 1.1: Core Domain Entities ✅
- [x] Phase 1.2: Infrastructure - Database Setup ✅
- [x] Phase 1.3: Repository Pattern Implementation ✅

**What I learned**:

**Phase 1.1 - Core Domain Entities:**
- Created BaseEntity abstract class with common properties (Id, CreatedAt, UpdatedAt, IsDeleted)
- Created enums for MetricType, PlatformType, and PullRequestStatus
- Created all domain entities: Developer, Repository, Commit, PullRequest, Metric
- Defined navigation properties for entity relationships
- Created IRepository<T> and IUnitOfWork interfaces
- Learned about CancellationToken for cancelling async operations
- Learned about Expression<Func<T, bool>> for flexible LINQ queries

**Phase 1.2 - Infrastructure & Database:**
- Created entity configurations using IEntityTypeConfiguration<T> for all 5 entities
- Used Fluent API to configure properties, constraints, and relationships
- Learned about `.HasMaxLength()`, `.IsRequired()`, `.HasConversion<string>()` for enums
- Configured indexes for better query performance (unique, composite, and regular indexes)
- Set up foreign key relationships with CASCADE delete behavior
- Configured many-to-many relationship between Developer and Repository
- Used `ApplyConfigurationsFromAssembly()` to auto-load all configurations
- Created comprehensive migration with `dotnet ef migrations add InitialDatabaseSchema`
- Applied migration to PostgreSQL database with `dotnet ef database update`
- Verified all 6 tables created: Developers, Repositories, Commits, PullRequests, Metrics, DeveloperRepository
- All indexes and foreign keys properly created in PostgreSQL

**Phase 1.3 - Repository Pattern:**
- Implemented generic `Repository<T>` class implementing `IRepository<T>` interface
- Used `DbSet<T>` and `context.Set<T>()` for dynamic entity access
- Learned about `AsNoTracking()` for read-only queries (better performance)
- Implemented `FindAsync()` with `Expression<Func<T, bool>>` for flexible queries
- Implemented `UnitOfWork` class implementing `IUnitOfWork` interface
- Used Dictionary to cache repositories (lazy loading pattern)
- Implemented transaction management: BeginTransaction, Commit, Rollback
- Learned about Dispose pattern and `GC.SuppressFinalize(this)` to prevent double cleanup
- Registered services in DI container using `AddScoped` lifetime
- Used generic DI registration `AddScoped(typeof(IRepository<>), typeof(Repository<>))`
- Understood DI lifetimes: Singleton, Scoped (per-request), Transient

**Time spent**: ~4 hours  
**Blockers**: None  
**Notes**: 
- Core layer is complete and builds successfully! All entities follow Clean Architecture principles.
- Database schema fully configured with proper relationships, indexes, and constraints
- Repository pattern fully implemented with Unit of Work for transaction management
- All services registered in DI container and ready to use
- Created feature branch for Phase 1.1: `feature/sprint1-phase1.1-core-entities` → Merged via PR #20
- Created feature branch for Phase 1.2: `sprint1/phase1.2-infrastructure-database-#21` → Merged via PR #21
- Created feature branch for Phase 1.3: `sprint1/phase1.3-repository-pattern-#22`
- Following professional git workflow: feature branch → commit → push → PR → merge
- Issue-driven development: Each phase has a GitHub issue

---

### Day 2 - October 19, 2025
**Phases completed**:
- [x] Phase 1.4: Logging & Error Handling ✅

**What I learned**:

**Phase 1.4 - Logging & Error Handling:**
- Updated `dotnet ef` tools from version 8.0.10 to 9.x.x to match project EF Core version
- Added Serilog.AspNetCore package (v9.0.0) which includes Console and File sinks
- Configured Serilog with `LoggerConfiguration()` to write to console and rolling files
- Used `RollingInterval.Day` for daily log files in `logs/devmetrics-log{Date}.txt`
- Learned about `Log.Logger` static logger configuration before application startup
- Wrapped application startup in try-catch-finally for proper error logging
- Used `Log.Fatal()` for application startup failures
- Used `Log.CloseAndFlush()` in finally block to ensure all logs are written before exit
- Learned that `CloseAndFlush()` prevents log loss by flushing buffered logs from memory to disk
- Implemented `IExceptionHandler` interface for global exception handling (ASP.NET Core 8+)
- Created `GlobalExceptionHandler` class to catch all unhandled exceptions
- Logged exceptions with `_logger.LogError()` for structured logging with full stack traces
- Returned JSON error responses with different detail levels for Dev vs Production
- Used `HttpStatusCode.InternalServerError` (500) for unhandled errors
- Registered exception handler with `AddExceptionHandler<GlobalExceptionHandler>()`
- Added `AddProblemDetails()` for RFC 7807 problem details support
- Used `app.UseExceptionHandler()` middleware in pipeline (early placement is important)
- Learned about `IHostEnvironment.IsDevelopment()` for environment-specific error messages
- Tested the handler - it successfully caught database connection errors and returned clean JSON
- Logs are automatically created in `logs/` directory (should be git-ignored)

**Time spent**: ~1.5 hours  
**Blockers**: None  
**Notes**: 
- Logging and error handling fully configured and tested!
- Serilog creates structured logs with timestamps and log levels
- All unhandled exceptions are now logged and return clean JSON responses
- Created feature branch: `sprint1/phase1.4-logging-error-handling-#26`
- Ready to commit and push 

---

### Day 2 - October 19, 2025 (Continued)
**Phases completed**:
- [x] Phase 1.4: Logging & Error Handling ✅
- [x] Phase 1.5: Week 1 Wrap-up ✅

**Phase 1.5 - Week 1 Wrap-up:**
- Created `DbInitializer.cs` static class with `SeedAsync()` method
- Added seed data for 3 test developers (Sarah Chen, Marcus Johnson, Lisa Wong)
- Configured automatic seeding in `Program.cs` for Development environment only
- Used `app.Services.CreateScope()` to create DI scope for seeding
- Learned about `context.Database.MigrateAsync()` to auto-apply migrations
- Understood difference between Transient, Scoped, and Singleton DI lifetimes
- Learned why manual scope creation is needed outside HTTP request context
- Ready to test once Docker is fully configured

**Week 1 Summary:**
- ✅ All 5 phases complete (1.1 through 1.5)
- ✅ Core domain layer complete with 5 entities
- ✅ Database fully configured with EF Core and PostgreSQL
- ✅ Repository pattern + Unit of Work implemented
- ✅ Logging with Serilog configured
- ✅ Global exception handler implemented
- ✅ Development seed data ready

**Time spent**: ~1.5 hours (Phase 1.5: ~0.5 hours)
**Week 1 total**: ~6 hours

---

### Day 3 - __________
**Phases completed**:
- [ ] Phase 1.3: Repository Pattern Implementation

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 4 - __________
**Phases completed**:
- [ ] Phase 1.4: Logging & Error Handling

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Blockers**: None / [describe]  
**Notes**: 

---

### Day 5 - __________
**Phases completed**:
- [ ] Phase 1.5: Week 1 Wrap-up

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Week 1 total**: ___ hours  
**Notes**: 

---

## WEEK 2: Authentication & Basic UI

### Day 6 - October 20, 2025
**Phases completed**:
- [x] Phase 1.6: ASP.NET Core Identity Setup ✅

**What I learned**:

**Phase 1.6 - ASP.NET Core Identity:**
- Created `ApplicationUser` entity extending `IdentityUser<Guid>` for custom user authentication
- Used `Guid` instead of default `string` for user IDs for better performance and consistency
- Added optional link between `ApplicationUser` and `Developer` entities (one user can be a developer)
- Added custom properties to ApplicationUser: `CreatedAt`, `LastLoginAt`, `DeveloperId`
- Updated `ApplicationDbContext` to extend `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`
- Learned that **calling `base.OnModelCreating(modelBuilder)` is CRITICAL** for Identity tables configuration
- Configured `SetNull` delete behavior (if developer deleted, user account remains)
- Configured Identity services in `Program.cs` with password policies:
  - Password: 8+ chars, requires digit, upper/lowercase (no special chars required)
  - Lockout: 5 failed attempts = 5 min lockout (brute-force protection)
  - Email: Must be unique, no confirmation required in dev mode
- Configured application cookie settings:
  - Login/logout paths, 7-day expiration, sliding expiration
- Added authentication middleware: `UseAuthentication()` and `UseAuthorization()`
- Learned middleware order is critical: HttpsRedirection → Authentication → Authorization → Antiforgery
- Created `AddIdentityTables` migration that adds 7 Identity tables:
  - AspNetUsers (with custom fields)
  - AspNetRoles, AspNetUserRoles
  - AspNetUserClaims, AspNetRoleClaims
  - AspNetUserLogins, AspNetUserTokens
- Discovered Web project needed `Microsoft.AspNetCore.Identity.UI` package for Identity services
- Applied migration successfully - all Identity tables created in PostgreSQL
- Verified both migrations in `__EFMigrationsHistory` table
- Application runs successfully with full Identity integration

**Key Concepts:**
- **Identity Framework**: ASP.NET Core's built-in authentication/authorization system
- **IdentityDbContext**: Special DbContext that configures Identity tables automatically
- **IdentityUser<TKey>**: Base class for user entities with authentication properties
- **IdentityRole<TKey>**: Base class for role entities (for role-based authorization)
- **Password Policies**: Enforce strong passwords to protect user accounts
- **Lockout**: Temporary account lock after failed login attempts (security feature)
- **Sliding Expiration**: Cookie lifetime extends with user activity (stays logged in)
- **Authentication vs Authorization**: Authentication = who you are, Authorization = what you can do

**Challenges:**
- Initial build error: Host was aborted during `builder.Build()`
- Root cause: Missing Identity package in Web project
- Solution: Added `Microsoft.AspNetCore.Identity.UI` package to Web project
- Also needed to add authentication middleware to pipeline

**Time spent**: ~2 hours  
**Blockers**: Docker/WSL2 setup (resolved), missing package (resolved)  
**Notes**: 
- Identity is now fully integrated and ready for JWT authentication (Phase 1.7)
- All Identity tables created successfully in PostgreSQL
- Authentication middleware properly configured
- Ready to build authentication API endpoints
- Week 2 is 50% complete! 

---

### Day 7 - October 20, 2025
**Phases completed**:
- [x] Phase 1.7: JWT Authentication ✅

**What I learned**:

**Phase 1.7 - JWT Authentication:**
- Created `IJwtService` interface in Application layer with token generation methods
- Implemented `JwtService` in Infrastructure layer for JWT token generation
- Added JWT packages: `System.IdentityModel.Tokens.Jwt` and `Microsoft.IdentityModel.Tokens`
- Learned about **JWT (JSON Web Tokens)**: Compact, URL-safe tokens for secure information transmission
- Understood **Claims-based authentication**: Tokens contain claims (key-value pairs) about the user
- Implemented token generation with user claims: NameIdentifier, Email, Username, Roles
- Used **SymmetricSecurityKey** for token signing (HMAC-SHA256 algorithm)
- Configured **SigningCredentials** to ensure tokens can't be tampered with
- Set token expiration to 60 minutes (configurable in appsettings)
- Implemented cryptographically secure refresh token generation using `RandomNumberGenerator`
- Configured JWT settings in appsettings.json:
  - **Key**: Minimum 32 characters for HMAC-SHA256 security
  - **Issuer**: Who creates the token (`http://localhost:5234`)
  - **Audience**: Who the token is for (also `http://localhost:5234`)
  - **ExpirationMinutes**: Token lifetime (60 minutes)
- Configured **dual authentication schemes**: Cookie (Blazor) + JWT (API)
- Used `AddAuthentication().AddJwtBearer()` to add JWT without overriding Cookie auth
- Configured **TokenValidationParameters** for secure token validation:
  - ValidateIssuer, ValidateAudience, ValidateLifetime, ValidateIssuerSigningKey
- Fixed URL mismatch: Changed from `https://localhost:5001` to `http://localhost:5234`
- Learned that multiple authentication schemes can coexist in same application

**Key Concepts:**
- **JWT Structure**: Header.Payload.Signature (Base64 encoded)
- **Claims**: Information about user stored in token (id, email, roles, etc.)
- **Signing Key**: Secret used to sign tokens (prevents tampering)
- **HMAC-SHA256**: Symmetric signing algorithm (same key signs and validates)
- **Token Expiration**: Tokens have limited lifetime for security
- **Refresh Tokens**: Long-lived tokens to get new access tokens
- **Bearer Token**: Sent in Authorization header: `Bearer <token>`
- **Issuer**: Entity that creates the token
- **Audience**: Entity the token is intended for
- **Stateless Authentication**: Server doesn't store session, all info in token

**Challenges:**
- Initial issue: Missing JWT packages in Infrastructure project
- Solution: Added `System.IdentityModel.Tokens.Jwt` and `Microsoft.IdentityModel.Tokens`
- URL configuration mismatch: appsettings had wrong port
- Solution: Updated to match actual application URL
- Authentication scheme conflict: JWT was overriding Cookie auth
- Solution: Used `AddAuthentication().AddJwtBearer()` to add as additional scheme

**Time spent**: ~1.5 hours  
**Blockers**: Missing packages (resolved), URL mismatch (resolved)  
**Notes**: 
- JWT authentication fully configured and working alongside Cookie auth
- Ready to create API endpoints that use JWT tokens (Phase 1.8)
- Both Blazor (Cookie) and API (JWT) authentication schemes now supported
- Application runs successfully with all authentication configured 

---

### Day 8 - October 20, 2025 (Continued)
**Phases completed**:
- [x] Phase 1.8: Authentication API Endpoints ✅

**What I learned**:

**Phase 1.8 - Authentication API Endpoints:**
- Created Auth DTOs in Application layer:
  - `RegisterRequest` with Email, Password, ConfirmPassword validation
  - `LoginRequest` with Email, Password, RememberMe fields
  - `AuthResponse` with Token, Email, DisplayName, ExpiresAt, RefreshToken
- Used **Data Annotations** for validation: `[Required]`, `[EmailAddress]`, `[Compare]`
- Created `AuthController` API controller with `[ApiController]` and `[Route("api/[controller]")]`
- Injected Identity services: `UserManager<ApplicationUser>`, `SignInManager<ApplicationUser>`
- Implemented **POST /api/auth/register** endpoint:
  - Validates model state automatically (thanks to `[ApiController]`)
  - Creates new `ApplicationUser` with email as username
  - Uses `UserManager.CreateAsync(user, password)` for secure password hashing
  - Returns JWT token immediately after successful registration
- Implemented **POST /api/auth/login** endpoint:
  - Finds user by email with `UserManager.FindByEmailAsync()`
  - Validates password with `SignInManager.CheckPasswordSignInAsync()`
  - Handles account lockout after failed attempts (security feature)
  - Updates `LastLoginAt` timestamp on successful login
  - Returns JWT token with user information
- Used `[ProducesResponseType]` attributes for OpenAPI/Swagger documentation
- Learned about **UserManager**: High-level API for user CRUD operations
- Learned about **SignInManager**: Handles password validation and sign-in logic
- Fixed role assignment issue: Commented out `AddToRoleAsync()` (roles not created yet)
- Refactored to remove **magic numbers**: Used `Jwt:ExpirationMinutes` from config
- Injected `IConfiguration` to read JWT expiration setting
- Tested endpoints with PowerShell `Invoke-RestMethod`:
  - ✅ Register: Successfully created user and returned JWT
  - ✅ Login: Successfully authenticated and returned JWT
- Decoded JWT token to verify claims:
  - User ID (nameidentifier)
  - Email (emailaddress)
  - Username (name)
  - Expiration (exp) - 60 minutes
  - Issuer and Audience
- Verified token expiration matches configuration (60 minutes)

**Key Concepts:**
- **API Controller**: Special controller for REST APIs (no views, JSON responses)
- **Model Validation**: Automatic validation with Data Annotations
- **UserManager<T>**: Identity service for user management (create, find, update)
- **SignInManager<T>**: Identity service for authentication (password check, lockout)
- **Password Hashing**: Identity automatically hashes passwords with PBKDF2
- **Account Lockout**: Temporary lock after failed login attempts (brute-force protection)
- **Bearer Token**: JWT sent in Authorization header for API authentication
- **Claims**: User information embedded in JWT (stateless authentication)
- **Model State**: ASP.NET Core validates incoming data automatically
- **Action Results**: `Ok()`, `BadRequest()`, `Unauthorized()` return proper HTTP status codes

**Challenges:**
- Initial 500 error: Role "User" doesn't exist yet
- Solution: Commented out role assignment for now (will implement role seeding later)
- `CheckPasswordSignInAsync` signature error: Had extra `RememberMe` parameter
- Solution: Removed incorrect parameter (RememberMe is for cookie auth, not password check)
- Magic number in expiration: Hardcoded 30 minutes instead of config value
- Solution: Injected IConfiguration and read `Jwt:ExpirationMinutes`
- Process locking during rebuild: App was still running
- Solution: Killed process with `taskkill /F /PID`

**Testing:**
- Created PowerShell test scripts for quick endpoint testing
- Successfully registered new user: `finaltest@devmetrics.com`
- Successfully logged in and received new JWT token
- Decoded JWT to verify all claims present and correct
- Verified token expiration time matches configuration
- Cleaned up test scripts after verification

**Time spent**: ~2 hours  
**Blockers**: Role seeding needed (minor - not blocking MVP)  
**Notes**: 
- Authentication API fully functional! ✅
- Both register and login endpoints working perfectly
- JWT tokens generated with correct claims and expiration
- Ready to move to Blazor UI (Phase 1.9)
- Note: Role management will be added in future phase
- Controllers properly use configuration instead of magic numbers
- Authentication flow is complete and secure 

---

### Day 9 - October 21, 2025
**Phases completed**:
- [x] Phase 1.9: Basic Blazor UI ✅

**What I learned about Blazor**:

**Phase 1.9 - Basic Blazor UI:**
- Added and configured **MudBlazor** component library for modern Material Design UI
- Created **Login page** with email/password form, validation, and error handling
- Created **Register page** with email/password/confirm password validation
- Built **AuthStateService** to manage JWT tokens in browser `localStorage`
  - `GetTokenAsync()` - Retrieve token from localStorage
  - `SaveTokenAsync()` - Store token in localStorage
  - `RemoveTokenAsync()` - Clear token on logout
  - `IsAuthenticatedAsync()` - Check if token exists and is not expired
  - `GetUserInfoAsync()` - Extract user claims (email, name, roles) from JWT
- Updated **MainLayout** to display user email and logout button when authenticated
- Created **NavMenu** with MudBlazor navigation links (Dashboard, Repositories, Developers, Metrics, Settings)
- Built **Dashboard/Home page** with stat cards and activity feed (mock data for now)
- Implemented full auth flow: Register → Login → Dashboard → Logout

**Key Concepts:**
- **Blazor Server**: Server-side rendering with SignalR for real-time updates
- **InteractiveServer RenderMode**: Required for client-side interactivity (button clicks, localStorage access)
- **IJSRuntime**: Blazor's JavaScript interop for calling browser APIs like localStorage
- **Component Lifecycle**: `OnInitializedAsync()`, `OnAfterRenderAsync()`, `OnParametersSetAsync()`
- **NavigationManager**: Programmatic navigation and `LocationChanged` event subscription
- **@inject**: Dependency injection in Razor components
- **@rendermode**: Controls how components are rendered (SSR vs Interactive)
- **MudBlazor Components**: MudLayout, MudAppBar, MudButton, MudTextField, MudCard, MudChip, etc.

**Challenges:**
- **MudDrawer JS Interop Error**: `mudElementRef.getBoundingClientRect` undefined error crashed Blazor circuit
- **Solution**: Replaced `MudDrawer` with simple `MudPaper` + CSS positioning (no JS required)
- **Auth State Not Updating**: MainLayout didn't refresh after login
- **Solution**: Subscribe to `Navigation.LocationChanged` to reload auth state on navigation
- **Circuit Crashes**: Unhandled JS errors kill ALL interactivity (buttons stop working)
- **Lesson**: Always check browser console AND server logs for circuit errors
- **Server-Side Prerendering**: During SSR, `localStorage` doesn't exist yet
- **Solution**: Set `@rendermode="RenderMode.InteractiveServer"` on `<Routes />` component

**Testing:**
- ✅ Register new user → Receives JWT token → Redirects to dashboard
- ✅ Login with existing user → Token stored → Email displayed in header
- ✅ Logout → Token removed → Redirects to login page
- ✅ Dashboard shows welcome message for unauthenticated users
- ✅ Dashboard shows stat cards for authenticated users

**Time spent**: ~4 hours  
**Blockers**: None  
**Notes**: 
- Authentication UI fully functional! ✅
- Complete register/login/logout flow working
- User email displays in navigation bar when authenticated
- MudBlazor provides beautiful Material Design components
- Ready for data integration (Phase 2.x) 

---

### Day 10 - __________
**Phases completed**:
- [ ] Phase 1.10: Sprint 1 Wrap-up

**What I learned**:
- 
- 

**Time spent**: ___ hours  
**Week 2 total**: ___ hours  
**Notes**: 

---

## 🎓 Learning Summary

### Blazor Concepts Learned
- 
- 

### .NET Core Concepts
- 
- 

### Database & EF Core
- 
- 

### Architecture Patterns
- 
- 

---

## 📈 Metrics

- **Total time spent**: ___ hours (estimated: 20-30h)
- **Commits made**: ___
- **Tests written**: ___
- **Test coverage**: ___%
- **Phases completed**: ___ / 10
- **Success criteria met**: ___ / ___

---

## ✅ Sprint Success Criteria

- [x] Core domain entities implemented
- [x] Entity Framework Core configured
- [x] Database migrations working
- [x] Repository pattern with Unit of Work
- [x] ASP.NET Core Identity setup ✅
- [x] JWT authentication functional ✅
- [x] Auth API endpoints (register/login) ✅
- [ ] Basic Blazor UI with MudBlazor
- [x] Logging configured
- [x] Error handling middleware
- [ ] >80% test coverage
- [ ] CI pipeline green
- [ ] Documentation updated

---

## 🔄 Sprint Retrospective

### What went well ✅
- 
- 

### What could be improved 🔄
- 
- 

### Blazor learning curve
- Easy parts:
- Challenging parts:

### Action items for Sprint 2 📝
- 
- 

### Velocity notes
- Estimated time: 20-30 hours
- Actual time: ___ hours
- Accuracy: ___% 

---

## 📸 Screenshots

_(Add screenshots of working UI, database, test results)_

---

## 🚀 Ready for Sprint 2?

- [ ] All Sprint 1 success criteria met
- [ ] No blockers remaining
- [ ] Authentication working end-to-end
- [ ] Comfortable with Blazor basics
- [ ] Documentation up to date

**Date completed**: ___________  
**Release tag**: v0.2-sprint1


