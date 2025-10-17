using DevMetricsPro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMetricsPro.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Repository entity
/// </summary>
public class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        // Table name
        builder.ToTable("Repositories");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.ExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Url)
            .HasMaxLength(500);

        builder.Property(r => r.DefaultBranch)
            .HasMaxLength(100);

        builder.Property(r => r.Platform)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string in database

        
        // Indexes for better query performance
        builder.HasIndex(r => r.ExternalId);
        builder.HasIndex(r => r.Platform);
        builder.HasIndex(r => r.IsActive);


        // Relationships
        builder.HasMany(r => r.Commits)
            .WithOne(c => c.Repository)
            .HasForeignKey(c => c.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.PullRequests)
            .WithOne(p => p.Repository)
            .HasForeignKey(p => p.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}