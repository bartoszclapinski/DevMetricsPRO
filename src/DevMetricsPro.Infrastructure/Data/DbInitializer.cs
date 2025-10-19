using DevMetricsPro.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevMetricsPro.Infrastructure.Data;

/// <summary>
/// Initializes the database with sample data
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Seeds the database with sample data
    /// </summary>
    /// <param name="context">The database context</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if we already have data - don't seed twice
        if (await context.Developers.AnyAsync()) return; 

        var developers = new[]
        {
            new Developer
            {
                Email = "sarah.chen@devmetricspro.com",
                DisplayName = "Sarah Chen",
                GitHubUsername = "sarahchen",
                AvatarUrl = "https://avatars.githubusercontent.com/u/1234567"
            },
            new Developer
            {
                Email = "marcus.johnson@devmetrics.com",
                DisplayName = "Marcus Johnson",
                GitHubUsername = "mjohnson",
                AvatarUrl = "https://avatars.githubusercontent.com/u/2345678"
            },
            new Developer
            {
                Email = "lisa.wong@devmetrics.com",
                DisplayName = "Lisa Wong",
                GitHubUsername = "lisawong",
                GitLabUsername = "lwong",
                AvatarUrl = "https://avatars.githubusercontent.com/u/3456789"
            }
        };

        // Add all developers to the context
        await context.Developers.AddRangeAsync(developers);

        // Save changes to database
        await context.SaveChangesAsync();
    }
}