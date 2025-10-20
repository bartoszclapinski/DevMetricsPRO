using DevMetricsPro.Web.Components;
using DevMetricsPro.Infrastructure.Data;
using DevMetricsPro.Core.Interfaces;
using DevMetricsPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using DevMetricsPro.Web.Middleware;
using DevMetricsPro.Core.Entities;
using Microsoft.AspNetCore.Identity;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/devmetrics-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
    
try 
{
    Log.Information("Starting DevMetrics Pro application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Exception handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Repository Pattern - Scoped lifetime for per-request instances
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

    // Authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
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

