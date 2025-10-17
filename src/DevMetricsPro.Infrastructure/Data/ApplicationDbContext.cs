using Microsoft.EntityFrameworkCore;
using DevMetricsPro.Core.Entities;

namespace DevMetricsPro.Infrastructure.Data;

/// <summary>
/// Main database context for the application
/// </summary>
public class ApplicationDbContext : DbContext
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

        // Apply all entity configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}