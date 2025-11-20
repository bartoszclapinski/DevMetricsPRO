namespace DevMetricsPro.Web.Configuration;

/// <summary>
/// Configuration for Cross-Origin Resource Sharing (CORS) policies
/// </summary>
public static class CorsConfiguration
{
    public const string DefaultPolicy = "DefaultCorsPolicy";

    /// <summary>
    /// Configures CORS policies for the application
    /// </summary>
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(DefaultPolicy, builder =>
            {
                // Get allowed origins from configuration
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                    ?? new[] { "http://localhost:5000", "https://localhost:5001" };

                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials() // Required for Blazor Server with SignalR
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
            });
        });

        return services;
    }
}

