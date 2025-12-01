using DevMetricsPro.Web.Components;
using DevMetricsPro.Infrastructure.Data;
using DevMetricsPro.Core.Interfaces;
using DevMetricsPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using DevMetricsPro.Web.Middleware;
using DevMetricsPro.Web.Configuration;
using DevMetricsPro.Core.Entities;
using Microsoft.AspNetCore.Identity;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevMetricsPro.Web.Services;
using Hangfire;
using Hangfire.PostgreSql;
using FluentValidation.AspNetCore;
using FluentValidation;
using DevMetricsPro.Application.Validators;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http.Features;
using DevMetricsPro.Application.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
    
try 
{
    var builder = WebApplication.CreateBuilder(args);

    var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
    {
        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = appInsightsConnectionString;
        });
    }

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/devmetrics-log.txt", rollingInterval: RollingInterval.Day);

        var telemetryConfiguration = services.GetService<TelemetryConfiguration>();
        if (!string.IsNullOrWhiteSpace(context.Configuration["ApplicationInsights:ConnectionString"]) &&
            telemetryConfiguration is not null)
        {
            loggerConfiguration.WriteTo.ApplicationInsights(
                telemetryConfiguration,
                TelemetryConverter.Traces);
        }
    });

    // Exception handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Distributed cache (Redis preferred, memory fallback) + session
    var redisConnection = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrWhiteSpace(redisConnection))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
        });
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(10);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
    });

    // Security: Rate limiting policies
    builder.Services.AddRateLimitingPolicies();

    // Security: CORS policies
    builder.Services.AddCorsPolicies(builder.Configuration);

    // Security: Request size limits
    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = builder.Configuration.GetValue<long>("Security:MaxMultipartBodyLength", 10485760); // 10 MB default
    });
    
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxRequestBodySize = builder.Configuration.GetValue<long>("Security:MaxRequestBodySize", 10485760); // 10 MB default
    });

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Repository Pattern - Scoped lifetime for per-request instances
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Chart data services
    builder.Services.AddScoped<IChartDataService, ChartDataService>();
    
    // Leaderboard services
    builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
    
    // Authentication state service
    builder.Services.AddScoped<AuthStateService>();

    // Identity Configuration
    builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Set to true for production
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Fluent Validation
    builder.Services
       .AddFluentValidationAutoValidation()
       .AddValidatorsFromAssemblyContaining<GitHubCallbackRequestValidator>();

    // GitHub OAuth Service
    builder.Services.AddHttpClient(); // Required for HttpClientFactory
    builder.Services.AddScoped<IGitHubOAuthService, GitHubOAuthService>();
    builder.Services.AddScoped<IGitHubRepositoryService, GitHubRepositoryService>();
    builder.Services.AddScoped<IGitHubCommitsService, GitHubCommitsService>();
    builder.Services.AddScoped<IGitHubPullRequestService, GitHubPullRequestService>();
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();

    // Metrics Calculation Service
    builder.Services.AddScoped<IMetricsCalculationService, MetricsCalculationService>();

    // Background Jobs
    builder.Services.AddScoped<DevMetricsPro.Web.Jobs.SyncGitHubDataJob>();

    // Configure application cookie
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

    builder.Services.AddMudServices();

    // API Controllers
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

    // HttpClient for calling our own API from Blazor components
    builder.Services.AddScoped(sp => new HttpClient 
    { 
        BaseAddress = new Uri("http://localhost:5234") 
    });

    // Jwt Service
    builder.Services.AddScoped<IJwtService, JwtService>();

    // JWT Authentication (additional scheme for API endpoints)
    builder.Services.AddAuthentication()
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

    // Hangfire Configuration
    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => 
            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

    // Add Hangfire server
    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = 5; // Number of concurrent background workers
        options.ServerName = "DevMetricsPro-Server";
    });

    var app = builder.Build();

    // Use exception handler middleware
    app.UseExceptionHandler();
    
    if (!app.Environment.IsDevelopment())
    {        
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context);
    }

    app.UseHttpsRedirection();

    // Security: Add security headers (CSP, X-Frame-Options, etc.)
    app.UseSecurityHeaders();

    // Security: Enable CORS
    app.UseCors(CorsConfiguration.DefaultPolicy);

    // Security: Enable rate limiting
    app.UseRateLimiter();

    // Add correlation ID to all requests for distributed tracing
    app.UseCorrelationId();

    // Log performance metrics for slow requests
    app.UsePerformanceLogging();

    app.UseSerilogRequestLogging();

    // Authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Hangfire Dashboard (only accessible to authenticated users)
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() },
        DashboardTitle = "DevMetrics Pro - Background Jobs"
    });

    // Schedule recurring jobs
    ConfigureRecurringJobs();

    app.UseSession();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.MapControllers();

    Log.Information("Starting DevMetrics Pro application");
    app.Run();

    // Local function to configure recurring jobs
    void ConfigureRecurringJobs()
    {
        // Note: For MVP, we're not scheduling automatic syncs yet
        // Users will manually trigger syncs from the UI or Hangfire dashboard
        // Future enhancement: Add recurring jobs for automatic data synchronization
        
        Log.Information("Hangfire recurring jobs configured (manual sync only for MVP)");
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

