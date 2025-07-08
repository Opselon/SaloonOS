using SaloonOS.Application.Common.Contracts;

namespace SaloonOS.Api.Services;

/// <summary>
/// Implements the ITenantContext for an HTTP-based application.
/// It uses the IHttpContextAccessor to retrieve the TenantId that was
/// populated by the ApiKeyAuthenticationMiddleware and stored in the HttpContext.Items collection.
/// This service should be registered with a Scoped lifetime.
/// </summary>
public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public Guid? TenantId => _httpContextAccessor.HttpContext?.Items["TenantId"] as Guid?;
}