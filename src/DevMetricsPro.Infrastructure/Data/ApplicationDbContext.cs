using Microsoft.EntityFrameworkCore;
using DevMetricsPro.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DevMetricsPro.Infrastructure.Data;

/// <summary>
/// Main database context for the application with Identity integration
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Developers table
    /// </summary>
    public DbSet<Developer> Developers { get; set; } = null!;

    /// <summary>
    /// Repositories table
    /// </summary>
    public DbSet<Repository> Repositories { get; set; } = null!;

    /// <summary>
    /// Commits table
    /// </summary>
    public DbSet<Commit> Commits { get; set; } = null!;

    /// <summary>
    /// Pull requests table
    /// </summary>
    public DbSet<PullRequest> PullRequests { get; set; } = null!;

    /// <summary>
    /// Metrics table
    /// </summary>
    public DbSet<Metric> Metrics { get; set; } = null!;

    /// <summary>
    /// Constructor for ApplicationDbContext
    /// </summary>
    /// <param name="options">Database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configure entity models using Fluent API
    /// </summary>
    /// <param name="modelBuilder">Model builder instance</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser relationships with Developer
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Developer)
            .WithOne()
            .HasForeignKey<ApplicationUser>(u => u.DeveloperId)
            .OnDelete(DeleteBehavior.SetNull);

        // Apply all entity configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Update timestamps automatically for modified entities
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

        foreach (var entry in entries) ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;

        return base.SaveChangesAsync(cancellationToken);
    }
}