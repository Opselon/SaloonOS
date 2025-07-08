using SaloonOS.Api.Middleware;

namespace SaloonOS.Api.Extensions;

/// <summary>
/// Provides a clean extension method to register our custom middleware
/// in the Program.cs file, improving readability and adhering to conventions.
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
    }
}