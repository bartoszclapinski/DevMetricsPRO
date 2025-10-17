using DevMetricsPro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMetricsPro.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Developer entity
/// </summary>
public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
{
    public void Configure(EntityTypeBuilder<Developer> builder)
    {
        // Table name
        builder.ToTable("Developers");

        // Primary key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.GitHubUsername)
            .HasMaxLength(100);

        builder.Property(d => d.GitLabUsername)
            .HasMaxLength(100);

        builder.Property(d => d.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.AvatarUrl)
            .HasMaxLength(500);


        // Indexes for better query performance
        builder.HasIndex(d => d.Email)
            .IsUnique();            

        builder.HasIndex(d => d.GitHubUsername);
        builder.HasIndex(d => d.GitLabUsername);

        
        // Relationships
        builder.HasMany(d => d.Repositories)
            .WithMany(r => r.Contributors);

        builder.HasMany(d => d.Commits)
            .WithOne(m => m.Developer)
            .HasForeignKey(m => m.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Metrics)
            .WithOne(m => m.Developer)
            .HasForeignKey(m => m.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}