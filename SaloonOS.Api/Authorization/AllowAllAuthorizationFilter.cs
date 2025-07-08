// Path: SaloonOS.Api/Authorization/AllowAllAuthorizationFilter.cs
using Hangfire.Dashboard;

namespace SaloonOS.Api.Authorization;

/// <summary>
/// An implementation of Hangfire's authorization filter that allows all requests.
/// WARNING: This filter is intended for DEVELOPMENT ENVIRONMENTS ONLY.
/// In a production environment, this should be replaced with a filter that checks
/// for specific user roles or claims to ensure only authorized administrators can
/// access the Hangfire dashboard.
/// </summary>
public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <summary>
    /// This method is called by Hangfire to determine if a request is authorized.
    /// By always returning true, we effectively disable authentication for the dashboard.
    /// </summary>
    /// <param name="context">The dashboard context, containing the HttpContext.</param>
    /// <returns>Always returns true.</returns>
    public bool Authorize(DashboardContext context)
    {
        // WARNING: Allowing all anonymous access.
        return true;
    }
}