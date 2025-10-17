using DevMetricsPro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMetricsPro.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Metric entity
/// </summary>
public class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> builder)
    {
        // Table name
        builder.ToTable("Metrics");

        // Primary key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.Property(m => m.Value)
            .IsRequired()
            .HasPrecision(18, 4); // Decimal with precision

        builder.Property(m => m.Timestamp)
            .IsRequired();

        builder.Property(m => m.Metadata)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(m => m.DeveloperId);
        builder.HasIndex(m => m.RepositoryId);
        builder.HasIndex(m => m.Type);
        builder.HasIndex(m => m.Timestamp);

        // Composite index for efficient queries
        builder.HasIndex(m => new { m.DeveloperId, m.Type, m.Timestamp });
    }
}