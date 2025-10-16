using Microsoft.EntityFrameworkCore;
using DevMetricsPro.Core.Entities;

namespace DevMetricsPro.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// The developers table
    /// </summary>
    public DbSet<Developer> Developers { get; set; } = null!;

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

        // TODO: Add entity configurations here in future phases
    }
}