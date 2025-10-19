using DevMetricsPro.Web.Components;
using DevMetricsPro.Infrastructure.Data;
using DevMetricsPro.Core.Interfaces;
using DevMetricsPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using DevMetricsPro.Web.Middleware;

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

    builder.Services.AddMudServices();

    var app = builder.Build();

    // Use exception handler middleware
    app.UseExceptionHandler();

    
    if (!app.Environment.IsDevelopment())
    {        
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();


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

