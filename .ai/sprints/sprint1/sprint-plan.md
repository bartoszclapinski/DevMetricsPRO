# Sprint 1: Foundation & Architecture 🏗️

**Duration**: 2 weeks (10 working days)  
**Goal**: Solid foundation with domain entities, database, authentication, and basic UI  
**Commitment**: 20-30 hours total (~2-3 hours/day)  

---

## 📋 Sprint Overview

This sprint establishes the core foundation of DevMetrics Pro:
- **Week 1**: Domain layer, database setup, data access layer
- **Week 2**: Authentication, authorization, basic Blazor UI

Each phase is small and testable, allowing incremental progress verification.

---

## 🎯 Sprint Goals

- ✅ Core domain entities defined
- ✅ Entity Framework Core configured
- ✅ Database migrations working
- ✅ Repository pattern implemented
- ✅ ASP.NET Core Identity setup
- ✅ JWT authentication working
- ✅ Basic Blazor UI with layout
- ✅ User registration and login functional

---

# WEEK 1: Core Setup & Data Layer

## Phase 1.1: Core Domain Entities (Day 1)

### Step 1.1: Create Base Entity
- [ ] **Create src/DevMetricsPro.Core/Entities/BaseEntity.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Entities;

  public abstract class BaseEntity
  {
      public Guid Id { get; set; } = Guid.NewGuid();
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public DateTime? UpdatedAt { get; set; }
      public bool IsDeleted { get; set; } = false;
  }
  ```

**✅ Test**: Build project - should compile

---

### Step 1.2: Create Enums
- [ ] **Create src/DevMetricsPro.Core/Enums/MetricType.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Enums;

  public enum MetricType
  {
      Commits,
      PullRequests,
      CodeReviews,
      IssuesClosed,
      LinesAdded,
      LinesRemoved,
      ActiveDays,
      AverageResponseTime
  }
  ```

- [ ] **Create src/DevMetricsPro.Core/Enums/PlatformType.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Enums;

  public enum PlatformType
  {
      GitHub,
      GitLab,
      Azure
  }
  ```

- [ ] **Create src/DevMetricsPro.Core/Enums/PullRequestStatus.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Enums;

  public enum PullRequestStatus
  {
      Open,
      Closed,
      Merged,
      Draft
  }
  ```

**✅ Test**: Build - enums compile correctly

---

### Step 1.3: Create Developer Entity
- [ ] **Create src/DevMetricsPro.Core/Entities/Developer.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Entities;

  public class Developer : BaseEntity
  {
      public string Email { get; set; } = string.Empty;
      public string? GitHubUsername { get; set; }
      public string? GitLabUsername { get; set; }
      public string? AvatarUrl { get; set; }
      public string? DisplayName { get; set; }

      // Navigation properties
      public ICollection<Repository> Repositories { get; set; } = new List<Repository>();
      public ICollection<Commit> Commits { get; set; } = new List<Commit>();
      public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
  }
  ```

**✅ Test**: Build successful

---

### Step 1.4: Create Repository Entity
- [ ] **Create src/DevMetricsPro.Core/Entities/Repository.cs**:
  ```csharp
  using DevMetricsPro.Core.Enums;

  namespace DevMetricsPro.Core.Entities;

  public class Repository : BaseEntity
  {
      public string Name { get; set; } = string.Empty;
      public string? Description { get; set; }
      public PlatformType Platform { get; set; }
      public string ExternalId { get; set; } = string.Empty;
      public string? Url { get; set; }
      public string? DefaultBranch { get; set; } = "main";
      public bool IsActive { get; set; } = true;
      public DateTime? LastSyncedAt { get; set; }

      // Navigation properties
      public ICollection<Developer> Contributors { get; set; } = new List<Developer>();
      public ICollection<Commit> Commits { get; set; } = new List<Commit>();
      public ICollection<PullRequest> PullRequests { get; set; } = new List<PullRequest>();
  }
  ```

**✅ Test**: Build successful

---

### Step 1.5: Create Commit Entity
- [ ] **Create src/DevMetricsPro.Core/Entities/Commit.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Entities;

  public class Commit : BaseEntity
  {
      public Guid RepositoryId { get; set; }
      public Guid DeveloperId { get; set; }
      public string Sha { get; set; } = string.Empty;
      public string Message { get; set; } = string.Empty;
      public int LinesAdded { get; set; }
      public int LinesRemoved { get; set; }
      public int FilesChanged { get; set; }
      public DateTime CommittedAt { get; set; }

      // Navigation properties
      public Repository Repository { get; set; } = null!;
      public Developer Developer { get; set; } = null!;
  }
  ```

**✅ Test**: Build successful

---

### Step 1.6: Create PullRequest Entity
- [ ] **Create src/DevMetricsPro.Core/Entities/PullRequest.cs**:
  ```csharp
  using DevMetricsPro.Core.Enums;

  namespace DevMetricsPro.Core.Entities;

  public class PullRequest : BaseEntity
  {
      public Guid RepositoryId { get; set; }
      public Guid AuthorId { get; set; }
      public string ExternalId { get; set; } = string.Empty;
      public string Title { get; set; } = string.Empty;
      public string? Description { get; set; }
      public PullRequestStatus Status { get; set; }
      public DateTime? MergedAt { get; set; }
      public DateTime? ClosedAt { get; set; }
      public int CommentsCount { get; set; }
      public int ChangedFilesCount { get; set; }

      // Navigation properties
      public Repository Repository { get; set; } = null!;
      public Developer Author { get; set; } = null!;
  }
  ```

**✅ Test**: Build successful

---

### Step 1.7: Create Metric Entity
- [ ] **Create src/DevMetricsPro.Core/Entities/Metric.cs**:
  ```csharp
  using DevMetricsPro.Core.Enums;

  namespace DevMetricsPro.Core.Entities;

  public class Metric : BaseEntity
  {
      public Guid DeveloperId { get; set; }
      public Guid? RepositoryId { get; set; }
      public MetricType Type { get; set; }
      public decimal Value { get; set; }
      public DateTime Timestamp { get; set; }
      public string? Metadata { get; set; } // JSON for additional data

      // Navigation properties
      public Developer Developer { get; set; } = null!;
      public Repository? Repository { get; set; }
  }
  ```

**✅ Test**: Run `dotnet build` - all entities compile

---

### Step 1.8: Create Core Interfaces
- [ ] **Create src/DevMetricsPro.Core/Interfaces/IRepository.cs**:
  ```csharp
  using System.Linq.Expressions;

  namespace DevMetricsPro.Core.Interfaces;

  public interface IRepository<T> where T : class
  {
      Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
      Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
      Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
      Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
      Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
      Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
      Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
  }
  ```

- [ ] **Create src/DevMetricsPro.Core/Interfaces/IUnitOfWork.cs**:
  ```csharp
  namespace DevMetricsPro.Core.Interfaces;

  public interface IUnitOfWork : IDisposable
  {
      IRepository<T> Repository<T>() where T : class;
      Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
      Task BeginTransactionAsync(CancellationToken cancellationToken = default);
      Task CommitTransactionAsync(CancellationToken cancellationToken = default);
      Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
  }
  ```

**✅ Test**: Build Core project successfully

---

### Step 1.9: Write Unit Tests for Entities
- [ ] **Create tests/DevMetricsPro.Core.Tests/Entities/DeveloperTests.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;
  using FluentAssertions;

  namespace DevMetricsPro.Core.Tests.Entities;

  public class DeveloperTests
  {
      [Fact]
      public void Developer_ShouldInitializeWithDefaults()
      {
          // Arrange & Act
          var developer = new Developer
          {
              Email = "test@example.com",
              DisplayName = "Test Developer"
          };

          // Assert
          developer.Id.Should().NotBeEmpty();
          developer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
          developer.IsDeleted.Should().BeFalse();
          developer.Email.Should().Be("test@example.com");
          developer.Repositories.Should().BeEmpty();
          developer.Commits.Should().BeEmpty();
      }
  }
  ```

- [ ] **Run tests**:
  ```bash
  dotnet test tests/DevMetricsPro.Core.Tests
  ```

**✅ Test**: All tests pass (green)

---

## Phase 1.2: Infrastructure - Database Setup (Day 2)

### Step 2.1: Add EF Core Packages
- [ ] **Add packages to Infrastructure project**:
  ```bash
  cd src/DevMetricsPro.Infrastructure
  dotnet add package Microsoft.EntityFrameworkCore
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
  dotnet add package Microsoft.EntityFrameworkCore.Tools
  cd ../..
  ```

**✅ Test**: Packages restore successfully

---

### Step 2.2: Create DbContext
- [ ] **Create src/DevMetricsPro.Infrastructure/Data/ApplicationDbContext.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;
  using Microsoft.EntityFrameworkCore;

  namespace DevMetricsPro.Infrastructure.Data;

  public class ApplicationDbContext : DbContext
  {
      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
      {
      }

      public DbSet<Developer> Developers => Set<Developer>();
      public DbSet<Repository> Repositories => Set<Repository>();
      public DbSet<Commit> Commits => Set<Commit>();
      public DbSet<PullRequest> PullRequests => Set<PullRequest>();
      public DbSet<Metric> Metrics => Set<Metric>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          base.OnModelCreating(modelBuilder);

          modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
      }

      public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      {
          // Update timestamps
          var entries = ChangeTracker.Entries()
              .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Modified));

          foreach (var entry in entries)
          {
              ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
          }

          return base.SaveChangesAsync(cancellationToken);
      }
  }
  ```

**✅ Test**: Build successful

---

### Step 2.3: Create Entity Configurations
- [ ] **Create src/DevMetricsPro.Infrastructure/Data/Configurations/DeveloperConfiguration.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;

  namespace DevMetricsPro.Infrastructure.Data.Configurations;

  public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
  {
      public void Configure(EntityTypeBuilder<Developer> builder)
      {
          builder.ToTable("Developers");

          builder.HasKey(d => d.Id);

          builder.Property(d => d.Email)
              .IsRequired()
              .HasMaxLength(255);

          builder.HasIndex(d => d.Email)
              .IsUnique();

          builder.Property(d => d.GitHubUsername)
              .HasMaxLength(100);

          builder.Property(d => d.GitLabUsername)
              .HasMaxLength(100);

          builder.Property(d => d.DisplayName)
              .HasMaxLength(200);

          builder.Property(d => d.AvatarUrl)
              .HasMaxLength(500);

          builder.HasMany(d => d.Commits)
              .WithOne(c => c.Developer)
              .HasForeignKey(c => c.DeveloperId)
              .OnDelete(DeleteBehavior.Cascade);

          builder.HasMany(d => d.Metrics)
              .WithOne(m => m.Developer)
              .HasForeignKey(m => m.DeveloperId)
              .OnDelete(DeleteBehavior.Cascade);

          // Soft delete filter
          builder.HasQueryFilter(d => !d.IsDeleted);
      }
  }
  ```

- [ ] **Create similar configurations for other entities**:
  - RepositoryConfiguration.cs
  - CommitConfiguration.cs
  - PullRequestConfiguration.cs
  - MetricConfiguration.cs

**✅ Test**: Build Infrastructure project

---

### Step 2.4: Configure DbContext in Web Project
- [ ] **Add package to Web project**:
  ```bash
  cd src/DevMetricsPro.Web
  dotnet add package Microsoft.EntityFrameworkCore.Design
  cd ../..
  ```

- [ ] **Update src/DevMetricsPro.Web/Program.cs** (add before `var app = builder.Build();`):
  ```csharp
  using DevMetricsPro.Infrastructure.Data;
  using Microsoft.EntityFrameworkCore;

  // ... existing code ...

  // Database
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseNpgsql(
          builder.Configuration.GetConnectionString("DefaultConnection"),
          b => b.MigrationsAssembly("DevMetricsPro.Infrastructure")));

  // ... rest of code ...
  ```

**✅ Test**: Application runs without errors

---

### Step 2.5: Create First Migration
- [ ] **Create initial migration**:
  ```bash
  dotnet ef migrations add InitialCreate -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web -o Data/Migrations
  ```

- [ ] **Review generated migration** in `src/DevMetricsPro.Infrastructure/Data/Migrations/`

- [ ] **Apply migration**:
  ```bash
  dotnet ef database update -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web
  ```

**✅ Test**: Check PostgreSQL - tables should exist:
```bash
docker exec -it devmetrics-postgres psql -U devmetrics -d devmetrics_dev -c "\dt"
```

---

## Phase 1.3: Repository Pattern Implementation (Day 3)

### Step 3.1: Create Generic Repository
- [ ] **Create src/DevMetricsPro.Infrastructure/Repositories/Repository.cs**:
  ```csharp
  using System.Linq.Expressions;
  using DevMetricsPro.Core.Interfaces;
  using DevMetricsPro.Infrastructure.Data;
  using Microsoft.EntityFrameworkCore;

  namespace DevMetricsPro.Infrastructure.Repositories;

  public class Repository<T> : IRepository<T> where T : class
  {
      protected readonly ApplicationDbContext _context;
      protected readonly DbSet<T> _dbSet;

      public Repository(ApplicationDbContext context)
      {
          _context = context;
          _dbSet = context.Set<T>();
      }

      public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
      {
          return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
      }

      public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
      {
          return await _dbSet.ToListAsync(cancellationToken);
      }

      public virtual async Task<IEnumerable<T>> FindAsync(
          Expression<Func<T, bool>> predicate,
          CancellationToken cancellationToken = default)
      {
          return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
      }

      public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
      {
          await _dbSet.AddAsync(entity, cancellationToken);
          return entity;
      }

      public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
      {
          _dbSet.Update(entity);
          return Task.CompletedTask;
      }

      public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
      {
          _dbSet.Remove(entity);
          return Task.CompletedTask;
      }

      public virtual async Task<int> CountAsync(
          Expression<Func<T, bool>>? predicate = null,
          CancellationToken cancellationToken = default)
      {
          return predicate == null
              ? await _dbSet.CountAsync(cancellationToken)
              : await _dbSet.CountAsync(predicate, cancellationToken);
      }
  }
  ```

**✅ Test**: Build successful

---

### Step 3.2: Create Unit of Work
- [ ] **Create src/DevMetricsPro.Infrastructure/Repositories/UnitOfWork.cs**:
  ```csharp
  using DevMetricsPro.Core.Interfaces;
  using DevMetricsPro.Infrastructure.Data;
  using Microsoft.EntityFrameworkCore.Storage;

  namespace DevMetricsPro.Infrastructure.Repositories;

  public class UnitOfWork : IUnitOfWork
  {
      private readonly ApplicationDbContext _context;
      private readonly Dictionary<Type, object> _repositories = new();
      private IDbContextTransaction? _transaction;

      public UnitOfWork(ApplicationDbContext context)
      {
          _context = context;
      }

      public IRepository<T> Repository<T>() where T : class
      {
          var type = typeof(T);

          if (!_repositories.ContainsKey(type))
          {
              var repositoryType = typeof(Repository<>).MakeGenericType(type);
              var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
              _repositories.Add(type, repositoryInstance!);
          }

          return (IRepository<T>)_repositories[type];
      }

      public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      {
          return await _context.SaveChangesAsync(cancellationToken);
      }

      public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
      {
          _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
      }

      public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
      {
          if (_transaction != null)
          {
              await _transaction.CommitAsync(cancellationToken);
              await _transaction.DisposeAsync();
              _transaction = null;
          }
      }

      public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
      {
          if (_transaction != null)
          {
              await _transaction.RollbackAsync(cancellationToken);
              await _transaction.DisposeAsync();
              _transaction = null;
          }
      }

      public void Dispose()
      {
          _transaction?.Dispose();
          _context.Dispose();
      }
  }
  ```

**✅ Test**: Build successful

---

### Step 3.3: Register Services
- [ ] **Update src/DevMetricsPro.Web/Program.cs**:
  ```csharp
  using DevMetricsPro.Core.Interfaces;
  using DevMetricsPro.Infrastructure.Repositories;

  // ... existing database configuration ...

  // Repositories
  builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
  builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
  ```

**✅ Test**: Application starts without DI errors

---

### Step 3.4: Write Integration Tests
- [ ] **Create tests/DevMetricsPro.Integration.Tests/Repositories/DeveloperRepositoryTests.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;
  using DevMetricsPro.Core.Interfaces;
  using DevMetricsPro.Infrastructure.Data;
  using DevMetricsPro.Infrastructure.Repositories;
  using FluentAssertions;
  using Microsoft.EntityFrameworkCore;

  namespace DevMetricsPro.Integration.Tests.Repositories;

  public class DeveloperRepositoryTests : IDisposable
  {
      private readonly ApplicationDbContext _context;
      private readonly IRepository<Developer> _repository;

      public DeveloperRepositoryTests()
      {
          var options = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;

          _context = new ApplicationDbContext(options);
          _repository = new Repository<Developer>(_context);
      }

      [Fact]
      public async Task AddAsync_ShouldAddDeveloper()
      {
          // Arrange
          var developer = new Developer
          {
              Email = "test@example.com",
              DisplayName = "Test Developer"
          };

          // Act
          await _repository.AddAsync(developer);
          await _context.SaveChangesAsync();

          // Assert
          var result = await _repository.GetByIdAsync(developer.Id);
          result.Should().NotBeNull();
          result!.Email.Should().Be("test@example.com");
      }

      [Fact]
      public async Task FindAsync_ShouldReturnMatchingDevelopers()
      {
          // Arrange
          var dev1 = new Developer { Email = "dev1@test.com", DisplayName = "Dev One" };
          var dev2 = new Developer { Email = "dev2@test.com", DisplayName = "Dev Two" };
          await _repository.AddAsync(dev1);
          await _repository.AddAsync(dev2);
          await _context.SaveChangesAsync();

          // Act
          var results = await _repository.FindAsync(d => d.Email.Contains("@test.com"));

          // Assert
          results.Should().HaveCount(2);
      }

      public void Dispose()
      {
          _context.Dispose();
      }
  }
  ```

- [ ] **Run integration tests**:
  ```bash
  dotnet test tests/DevMetricsPro.Integration.Tests
  ```

**✅ Test**: All integration tests pass

---

## Phase 1.4: Logging & Error Handling (Day 4)

### Step 4.1: Add Serilog
- [ ] **Add packages to Web project**:
  ```bash
  cd src/DevMetricsPro.Web
  dotnet add package Serilog.AspNetCore
  dotnet add package Serilog.Sinks.Console
  dotnet add package Serilog.Sinks.File
  cd ../..
  ```

### Step 4.2: Configure Serilog
- [ ] **Update src/DevMetricsPro.Web/Program.cs** (at the very top):
  ```csharp
  using Serilog;

  Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Information()
      .WriteTo.Console()
      .WriteTo.File("logs/devmetrics-.txt", rollingInterval: RollingInterval.Day)
      .CreateLogger();

  try
  {
      Log.Information("Starting DevMetrics Pro application");

      var builder = WebApplication.CreateBuilder(args);

      builder.Host.UseSerilog();

      // ... rest of configuration ...
  }
  catch (Exception ex)
  {
      Log.Fatal(ex, "Application failed to start");
      throw;
  }
  finally
  {
      Log.CloseAndFlush();
  }
  ```

**✅ Test**: Run app, check logs/devmetrics-*.txt file created

---

### Step 4.3: Create Global Exception Handler
- [ ] **Create src/DevMetricsPro.Web/Middleware/GlobalExceptionHandler.cs**:
  ```csharp
  using Microsoft.AspNetCore.Diagnostics;
  using System.Net;

  namespace DevMetricsPro.Web.Middleware;

  public class GlobalExceptionHandler : IExceptionHandler
  {
      private readonly ILogger<GlobalExceptionHandler> _logger;

      public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
      {
          _logger = logger;
      }

      public async ValueTask<bool> TryHandleAsync(
          HttpContext httpContext,
          Exception exception,
          CancellationToken cancellationToken)
      {
          _logger.LogError(exception, "An unhandled exception occurred");

          httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

          await httpContext.Response.WriteAsJsonAsync(new
          {
              error = "An error occurred processing your request",
              detail = httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                  ? exception.Message
                  : null
          }, cancellationToken);

          return true;
      }
  }
  ```

- [ ] **Register in Program.cs**:
  ```csharp
  builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
  builder.Services.AddProblemDetails();

  // ... later, after var app = builder.Build(); ...
  app.UseExceptionHandler();
  ```

**✅ Test**: Throw exception in a test endpoint, verify it's caught

---

## Phase 1.5: Week 1 Wrap-up (Day 5)

### Step 5.1: Add Seed Data for Development
- [ ] **Create src/DevMetricsPro.Infrastructure/Data/DbInitializer.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;
  using DevMetricsPro.Core.Enums;
  using Microsoft.EntityFrameworkCore;

  namespace DevMetricsPro.Infrastructure.Data;

  public static class DbInitializer
  {
      public static async Task SeedAsync(ApplicationDbContext context)
      {
          if (await context.Developers.AnyAsync())
          {
              return; // Already seeded
          }

          var developers = new[]
          {
              new Developer
              {
                  Email = "sarah.chen@example.com",
                  DisplayName = "Sarah Chen",
                  GitHubUsername = "sarahchen"
              },
              new Developer
              {
                  Email = "marcus.johnson@example.com",
                  DisplayName = "Marcus Johnson",
                  GitHubUsername = "mjohnson"
              }
          };

          await context.Developers.AddRangeAsync(developers);
          await context.SaveChangesAsync();
      }
  }
  ```

- [ ] **Call seeder in Program.cs** (after app.Build(), in Development only):
  ```csharp
  if (app.Environment.IsDevelopment())
  {
      using var scope = app.Services.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      await context.Database.MigrateAsync();
      await DbInitializer.SeedAsync(context);
  }
  ```

**✅ Test**: Run app, check database has seed data

---

### Step 5.2: Commit Week 1 Progress
- [ ] **Commit changes**:
  ```bash
  git add .
  git commit -m "feat: implement core domain entities and data layer"
  git push origin main
  ```

**✅ Test**: CI pipeline passes

---

# WEEK 2: Authentication & Basic UI

## Phase 1.6: ASP.NET Core Identity Setup (Day 6)

### Step 6.1: Add Identity Packages
- [ ] **Add to Infrastructure**:
  ```bash
  cd src/DevMetricsPro.Infrastructure
  dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
  cd ../..
  ```

### Step 6.2: Create ApplicationUser
- [ ] **Create src/DevMetricsPro.Core/Entities/ApplicationUser.cs**:
  ```csharp
  using Microsoft.AspNetCore.Identity;

  namespace DevMetricsPro.Core.Entities;

  public class ApplicationUser : IdentityUser<Guid>
  {
      public Guid? DeveloperId { get; set; }
      public Developer? Developer { get; set; }
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public DateTime? LastLoginAt { get; set; }
  }
  ```

**✅ Test**: Build successful

---

### Step 6.3: Update DbContext for Identity
- [ ] **Update ApplicationDbContext.cs**:
  ```csharp
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
  {
      // ... existing code ...

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          base.OnModelCreating(modelBuilder);

          // Configure Identity tables
          modelBuilder.Entity<ApplicationUser>()
              .HasOne(u => u.Developer)
              .WithOne()
              .HasForeignKey<ApplicationUser>(u => u.DeveloperId)
              .OnDelete(DeleteBehavior.SetNull);

          modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
      }
  }
  ```

### Step 6.4: Create Migration
- [ ] **Create and apply migration**:
  ```bash
  dotnet ef migrations add AddIdentity -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web
  dotnet ef database update -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web
  ```

**✅ Test**: Check database - Identity tables created

---

### Step 6.5: Configure Identity in Program.cs
- [ ] **Add to Program.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;
  using Microsoft.AspNetCore.Identity;

  // After DbContext configuration...

  // Identity
  builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
  {
      // Password settings
      options.Password.RequireDigit = true;
      options.Password.RequireLowercase = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequiredLength = 8;

      // Lockout settings
      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
      options.Lockout.MaxFailedAccessAttempts = 5;

      // User settings
      options.User.RequireUniqueEmail = true;
  })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

  builder.Services.ConfigureApplicationCookie(options =>
  {
      options.LoginPath = "/login";
      options.LogoutPath = "/logout";
      options.AccessDeniedPath = "/access-denied";
      options.ExpireTimeSpan = TimeSpan.FromDays(7);
      options.SlidingExpiration = true;
  });
  ```

**✅ Test**: Application builds and runs

---

## Phase 1.7: JWT Authentication (Day 7)

### Step 7.1: Add JWT Packages
- [ ] **Add to Web project**:
  ```bash
  cd src/DevMetricsPro.Web
  dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
  cd ../..
  ```

### Step 7.2: Create JWT Service Interface
- [ ] **Create src/DevMetricsPro.Application/Interfaces/IJwtService.cs**:
  ```csharp
  using DevMetricsPro.Core.Entities;

  namespace DevMetricsPro.Application.Interfaces;

  public interface IJwtService
  {
      string GenerateToken(ApplicationUser user, IList<string> roles);
      Task<string> GenerateRefreshTokenAsync();
  }
  ```

### Step 7.3: Implement JWT Service
- [ ] **Create src/DevMetricsPro.Infrastructure/Services/JwtService.cs**:
  ```csharp
  using System.IdentityModel.Tokens.Jwt;
  using System.Security.Claims;
  using System.Security.Cryptography;
  using System.Text;
  using DevMetricsPro.Application.Interfaces;
  using DevMetricsPro.Core.Entities;
  using Microsoft.Extensions.Configuration;
  using Microsoft.IdentityModel.Tokens;

  namespace DevMetricsPro.Infrastructure.Services;

  public class JwtService : IJwtService
  {
      private readonly IConfiguration _configuration;

      public JwtService(IConfiguration configuration)
      {
          _configuration = configuration;
      }

      public string GenerateToken(ApplicationUser user, IList<string> roles)
      {
          var claims = new List<Claim>
          {
              new(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new(ClaimTypes.Email, user.Email!),
              new(ClaimTypes.Name, user.UserName!)
          };

          claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

          var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
          var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
          var expires = DateTime.UtcNow.AddMinutes(
              Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]));

          var token = new JwtSecurityToken(
              issuer: _configuration["Jwt:Issuer"],
              audience: _configuration["Jwt:Audience"],
              claims: claims,
              expires: expires,
              signingCredentials: credentials
          );

          return new JwtSecurityTokenHandler().WriteToken(token);
      }

      public Task<string> GenerateRefreshTokenAsync()
      {
          var randomNumber = new byte[32];
          using var rng = RandomNumberGenerator.Create();
          rng.GetBytes(randomNumber);
          return Task.FromResult(Convert.ToBase64String(randomNumber));
      }
  }
  ```

### Step 7.4: Configure JWT in Program.cs
- [ ] **Add to Program.cs**:
  ```csharp
  using Microsoft.AspNetCore.Authentication.JwtBearer;
  using Microsoft.IdentityModel.Tokens;
  using System.Text;
  using DevMetricsPro.Application.Interfaces;
  using DevMetricsPro.Infrastructure.Services;

  // ... after Identity configuration ...

  // JWT Authentication
  builder.Services.AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
      };
  });

  builder.Services.AddScoped<IJwtService, JwtService>();

  // ... later, after var app = builder.Build(); ...
  app.UseAuthentication();
  app.UseAuthorization();
  ```

**✅ Test**: Application runs, authentication middleware registered

---

## Phase 1.8: Authentication API Endpoints (Day 8)

### Step 8.1: Create DTOs
- [ ] **Create src/DevMetricsPro.Application/DTOs/Auth/RegisterRequest.cs**:
  ```csharp
  using System.ComponentModel.DataAnnotations;

  namespace DevMetricsPro.Application.DTOs.Auth;

  public class RegisterRequest
  {
      [Required]
      [EmailAddress]
      public string Email { get; set; } = string.Empty;

      [Required]
      [MinLength(8)]
      public string Password { get; set; } = string.Empty;

      [Required]
      [Compare(nameof(Password))]
      public string ConfirmPassword { get; set; } = string.Empty;

      public string? DisplayName { get; set; }
  }
  ```

- [ ] **Create LoginRequest.cs, AuthResponse.cs** similarly

### Step 8.2: Create Auth API Controller
- [ ] **Create src/DevMetricsPro.Web/Controllers/AuthController.cs**:
  ```csharp
  using DevMetricsPro.Application.DTOs.Auth;
  using DevMetricsPro.Application.Interfaces;
  using DevMetricsPro.Core.Entities;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;

  namespace DevMetricsPro.Web.Controllers;

  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
      private readonly UserManager<ApplicationUser> _userManager;
      private readonly SignInManager<ApplicationUser> _signInManager;
      private readonly IJwtService _jwtService;
      private readonly ILogger<AuthController> _logger;

      public AuthController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IJwtService jwtService,
          ILogger<AuthController> logger)
      {
          _userManager = userManager;
          _signInManager = signInManager;
          _jwtService = jwtService;
          _logger = logger;
      }

      [HttpPost("register")]
      public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
      {
          var user = new ApplicationUser
          {
              UserName = request.Email,
              Email = request.Email
          };

          var result = await _userManager.CreateAsync(user, request.Password);

          if (!result.Succeeded)
          {
              return BadRequest(result.Errors);
          }

          await _userManager.AddToRoleAsync(user, "User");

          var roles = await _userManager.GetRolesAsync(user);
          var token = _jwtService.GenerateToken(user, roles);

          _logger.LogInformation("User {Email} registered successfully", user.Email);

          return Ok(new AuthResponse
          {
              Token = token,
              Email = user.Email!
          });
      }

      [HttpPost("login")]
      public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
      {
          var user = await _userManager.FindByEmailAsync(request.Email);

          if (user == null)
          {
              return Unauthorized("Invalid credentials");
          }

          var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

          if (!result.Succeeded)
          {
              return Unauthorized("Invalid credentials");
          }

          user.LastLoginAt = DateTime.UtcNow;
          await _userManager.UpdateAsync(user);

          var roles = await _userManager.GetRolesAsync(user);
          var token = _jwtService.GenerateToken(user, roles);

          _logger.LogInformation("User {Email} logged in", user.Email);

          return Ok(new AuthResponse
          {
              Token = token,
              Email = user.Email!
          });
      }
  }
  ```

- [ ] **Register controllers in Program.cs**:
  ```csharp
  builder.Services.AddControllers();
  
  // ... later ...
  app.MapControllers();
  ```

**✅ Test**: Test with Postman/curl:
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test1234!","confirmPassword":"Test1234!"}'
```

---

## Phase 1.9: Basic Blazor UI (Day 9)

### Step 9.1: Add MudBlazor
- [ ] **Add package**:
  ```bash
  cd src/DevMetricsPro.Web
  dotnet add package MudBlazor
  cd ../..
  ```

### Step 9.2: Configure MudBlazor
- [ ] **Update Program.cs**:
  ```csharp
  using MudBlazor.Services;

  builder.Services.AddMudServices();
  ```

- [ ] **Update _Imports.razor**:
  ```razor
  @using MudBlazor
  ```

- [ ] **Update App.razor**:
  ```razor
  <!DOCTYPE html>
  <html lang="en">
  <head>
      <meta charset="utf-8" />
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
      <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
      <HeadOutlet />
  </head>
  <body>
      <Routes />
      <script src="_content/MudBlazor/MudBlazor.min.js"></script>
  </body>
  </html>
  ```

### Step 9.3: Create Layout
- [ ] **Update Components/Layout/MainLayout.razor**:
  ```razor
  @inherits LayoutComponentBase

  <MudThemeProvider />
  <MudDialogProvider />
  <MudSnackbarProvider />

  <MudLayout>
      <MudAppBar Elevation="1">
          <MudIconButton Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" Edge="Edge.Start" />
          <MudText Typo="Typo.h6">DevMetrics Pro</MudText>
          <MudSpacer />
          <MudIconButton Icon="@Icons.Material.Filled.Brightness4" Color="Color.Inherit" />
      </MudAppBar>

      <MudDrawer Open="true" Elevation="1">
          <NavMenu />
      </MudDrawer>

      <MudMainContent>
          <MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-4">
              @Body
          </MudContainer>
      </MudMainContent>
  </MudLayout>
  ```

### Step 9.4: Create NavMenu
- [ ] **Update Components/Layout/NavMenu.razor**:
  ```razor
  <MudNavMenu>
      <MudNavLink Href="/" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Dashboard">
          Dashboard
      </MudNavLink>
      <MudNavLink Href="/repositories" Icon="@Icons.Material.Filled.Code">
          Repositories
      </MudNavLink>
      <MudNavLink Href="/developers" Icon="@Icons.Material.Filled.People">
          Developers
      </MudNavLink>
      <MudNavLink Href="/metrics" Icon="@Icons.Material.Filled.BarChart">
          Metrics
      </MudNavLink>
  </MudNavMenu>
  ```

### Step 9.5: Create Dashboard Page
- [ ] **Create Components/Pages/Dashboard.razor**:
  ```razor
  @page "/"
  @rendermode InteractiveServer

  <PageTitle>Dashboard - DevMetrics Pro</PageTitle>

  <MudText Typo="Typo.h4" Class="mb-4">Dashboard</MudText>

  <MudGrid>
      <MudItem xs="12" sm="6" md="3">
          <MudCard Elevation="2">
              <MudCardContent>
                  <MudText Typo="Typo.subtitle2" Color="Color.Secondary">Total Commits</MudText>
                  <MudText Typo="Typo.h3">1,247</MudText>
                  <MudText Typo="Typo.body2" Color="Color.Success">
                      <MudIcon Icon="@Icons.Material.Filled.TrendingUp" Size="Size.Small" /> 12.5% from last week
                  </MudText>
              </MudCardContent>
          </MudCard>
      </MudItem>

      <MudItem xs="12" sm="6" md="3">
          <MudCard Elevation="2">
              <MudCardContent>
                  <MudText Typo="Typo.subtitle2" Color="Color.Secondary">Pull Requests</MudText>
                  <MudText Typo="Typo.h3">89</MudText>
                  <MudText Typo="Typo.body2" Color="Color.Success">
                      <MudIcon Icon="@Icons.Material.Filled.TrendingUp" Size="Size.Small" /> 8.3% from last week
                  </MudText>
              </MudCardContent>
          </MudCard>
      </MudItem>

      <!-- Add more stat cards -->
  </MudGrid>

  <MudGrid Class="mt-4">
      <MudItem xs="12">
          <MudCard Elevation="2">
              <MudCardHeader>
                  <CardHeaderContent>
                      <MudText Typo="Typo.h6">Activity Chart</MudText>
                  </CardHeaderContent>
              </MudCardHeader>
              <MudCardContent>
                  <MudText Typo="Typo.body2" Color="Color.Secondary">
                      Chart will be implemented in Sprint 3
                  </MudText>
              </MudCardContent>
          </MudCard>
      </MudItem>
  </MudGrid>

  @code {
      // Will add logic in future sprints
  }
  ```

**✅ Test**: Run app, navigate to http://localhost:5000 - should see MudBlazor dashboard

---

## Phase 1.10: Sprint 1 Wrap-up (Day 10)

### Step 10.1: Update Documentation
- [ ] **Create .ai/sprints/sprint1-log.md** with completion status

### Step 10.2: Run Full Test Suite
- [ ] **Run all tests**:
  ```bash
  dotnet test
  ```

**✅ Test**: All tests pass

---

### Step 10.3: Code Coverage Report
- [ ] **Generate coverage**:
  ```bash
  dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
  ```

**✅ Goal**: >80% coverage on Core and Application projects

---

### Step 10.4: Final Commits
- [ ] **Commit all Week 2 changes**:
  ```bash
  git add .
  git commit -m "feat: implement authentication and basic Blazor UI"
  git push origin main
  ```

### Step 10.5: Create Release Tag
- [ ] **Tag Sprint 1 completion**:
  ```bash
  git tag -a v0.2-sprint1 -m "Sprint 1: Foundation & Architecture complete"
  git push origin v0.2-sprint1
  ```

**✅ Test**: CI pipeline passes, tag on GitHub

---

### Step 10.6: Update README
- [ ] **Update progress in README.md**:
  ```markdown
  ## 📝 Development Status
  - [x] Sprint 0: Setup ✅
  - [x] Sprint 1: Foundation & Auth ✅
  - [ ] Sprint 2: GitHub Integration (Next)
  ```

---

### Step 10.7: Sprint Retrospective
- [ ] **Document in sprint1-log.md**:
  - What went well?
  - What could be improved?
  - Action items for Sprint 2
  - Velocity achieved

---

## 🎯 Sprint 1 Success Criteria

- [x] Core domain entities implemented
- [x] Entity Framework Core configured
- [x] Database migrations working
- [x] Repository pattern with Unit of Work
- [x] ASP.NET Core Identity setup
- [x] JWT authentication functional
- [x] Auth API endpoints (register/login)
- [x] Basic Blazor UI with MudBlazor
- [x] Logging configured
- [x] Error handling middleware
- [x] >80% test coverage
- [x] CI pipeline green
- [x] Documentation updated

---

## 📊 Time Tracking

| Phase | Estimated Time | Actual Time |
|-------|---------------|-------------|
| 1.1 - Domain Entities | 3 hours | |
| 1.2 - Database Setup | 2 hours | |
| 1.3 - Repository Pattern | 2 hours | |
| 1.4 - Logging & Errors | 2 hours | |
| 1.5 - Week 1 Wrap | 1 hour | |
| 1.6 - Identity Setup | 2 hours | |
| 1.7 - JWT Auth | 2 hours | |
| 1.8 - Auth API | 3 hours | |
| 1.9 - Blazor UI | 3 hours | |
| 1.10 - Wrap-up | 2 hours | |
| **Total** | **22 hours** | |

---

## 🚀 Ready for Sprint 2!

Sprint 2 will focus on:
- GitHub API integration
- OAuth flow for GitHub
- Repository synchronization
- Background jobs with Hangfire
- Redis caching

**Great work on completing Sprint 1!** 🎉

