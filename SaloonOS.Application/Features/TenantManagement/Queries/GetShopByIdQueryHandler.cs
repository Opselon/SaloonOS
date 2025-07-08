using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.TenantManagement.Queries;

/// <summary>
/// Handles the logic for the GetShopByIdQuery. This is a prime example of a secure,
/// multi-tenant query handler. It not only fetches the data but also enforces
/// business rules and authorization.
/// </summary>
public class GetShopByIdQueryHandler : IRequestHandler<GetShopByIdQuery, ShopDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetShopByIdQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ShopDto?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Authorization: Ensure a tenant is authenticated.
        var authenticatedTenantId = _tenantContext.TenantId;
        if (authenticatedTenantId is null)
        {
            // This case should theoretically be blocked by the middleware, but defense-in-depth is key.
            throw new UnauthorizedAccessException();
        }

        // 2. Business Rule: A tenant can only request their own shop's details.
        if (authenticatedTenantId.Value != request.ShopId)
        {
            // This is a Forbidden action, not an Unauthorized one. The user is authenticated
            // but is trying to access a resource they do not own.
            // In a real API, you would throw a specific exception caught by middleware to return a 403 Forbidden.
            throw new InvalidOperationException("ForbiddenAccess"); // We'll use this string to lookup the localized message.
        }

        // 3. Data Retrieval: Fetch the entity from the database.
        var shop = await _unitOfWork.Shops.GetByIdAsync(request.ShopId);

        if (shop is null)
        {
            return null; // The controller will handle turning this into a 404 Not Found.
        }

        // 4. Mapping: Map the domain entity to the DTO. NEVER return the domain entity directly.
        return new ShopDto
        {
            Id = shop.Id,
            Name = shop.Name,
            DefaultLanguageCode = shop.DefaultLanguageCode,
            CreatedAt = shop.CreatedAt
        };
    }
}