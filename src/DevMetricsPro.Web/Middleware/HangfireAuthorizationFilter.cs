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
        // TODO: In production, require authentication
        #if DEBUG
            return true;
        #else
            return httpContext.User.Identity?.IsAuthenticated ?? false;
        #endif
    }
}

