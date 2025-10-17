using DevMetricsPro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMetricsPro.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Commit entity
/// </summary>
public class CommitConfiguration : IEntityTypeConfiguration<Commit>
{
    public void Configure(EntityTypeBuilder<Commit> builder)
    {
        // Table name
        builder.ToTable("Commits");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Sha)
            .IsRequired()
            .HasMaxLength(40); // Git SHA-1 is 40 characters

        builder.Property(c => c.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.CommittedAt)
            .IsRequired();

        
        // Indexes for better query performance
        builder.HasIndex(c => c.Sha)
            .IsUnique();

        builder.HasIndex(c => c.RepositoryId);
        builder.HasIndex(c => c.DeveloperId);
        builder.HasIndex(c => c.CommittedAt);
    }
}