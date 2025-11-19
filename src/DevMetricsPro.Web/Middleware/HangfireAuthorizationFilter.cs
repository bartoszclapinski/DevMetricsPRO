using Hangfire.Dashboard;

namespace DevMetricsPro.Web.Middleware;

/// <summary>
/// Authorization filter for Hangfire Dashboard.
/// In development: allows all access
/// In production: requires authentication
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // In development, allow all access for testing
        // Future enhancement: In production, require authentication for Hangfire dashboard
        #if DEBUG
            return true;
        #else
            return httpContext.User.Identity?.IsAuthenticated ?? false;
        #endif
    }
}

