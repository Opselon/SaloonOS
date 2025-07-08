using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Localization;
using SaloonOS.Api.Resources;
using SaloonOS.Application.Common.Contracts;

namespace SaloonOS.Api.Middleware;

/// <summary>
/// A powerful middleware responsible for validating the tenant's API key.
/// It intercepts incoming requests, extracts the 'X-Api-Key' header, hashes it,
/// and validates it against the database via the Unit of Work. If valid, it populates
/// the HttpContext with the TenantId for downstream use. If invalid, it short-circuits
/// the request with a localized 401 Unauthorized response.
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStringLocalizer<SharedResources> _localizer;
    public const string ApiKeyHeaderName = "X-Api-Key";

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, IStringLocalizer<SharedResources> localizer)
    {
        _next = next;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(_localizer["ApiKeyMissing"]);
            return;
        }

        var hashedApiKey = HashApiKey(extractedApiKey!);

        var shop = await unitOfWork.Shops.GetByHashedApiKeyAsync(hashedApiKey);

        if (shop is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(_localizer["ApiKeyInvalid"]);
            return;
        }

        // Key is valid. Attach the tenant ID to the context for this request.
        context.Items["TenantId"] = shop.Id;

        // Pass control to the next middleware in the pipeline.
        await _next(context);
    }

    // This must use the exact same hashing algorithm as the CreateShopCommandHandler.
    private static string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(apiKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}