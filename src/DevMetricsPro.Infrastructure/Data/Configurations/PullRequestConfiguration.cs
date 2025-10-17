using DevMetricsPro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMetricsPro.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PullRequest entity
/// </summary>
public class PullRequestConfiguration : IEntityTypeConfiguration<PullRequest>
{
    public void Configure(EntityTypeBuilder<PullRequest> builder)
    {
        // Table name
        builder.ToTable("PullRequests");

        // Primary key
        builder.HasKey(pr => pr.Id);

        // Properties
        builder.Property(pr => pr.ExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pr => pr.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pr => pr.Description)
            .HasMaxLength(5000);

        builder.Property(pr => pr.Status)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        // Indexes
        builder.HasIndex(pr => pr.ExternalId);
        builder.HasIndex(pr => pr.RepositoryId);
        builder.HasIndex(pr => pr.AuthorId);
        builder.HasIndex(pr => pr.Status);
    }
}