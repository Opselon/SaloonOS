using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.DTOs.ReadModels; // <-- ADD THIS USING for ShopReadModel

namespace SaloonOS.Application.Features.TenantManagement.Queries;

/// <summary>
/// Handles the logic for the GetShopByIdQuery. This is a pure read-side handler
/// that retrieves data from a fast, denormalized read store (e.g., Redis).
/// It enforces authorization but does NOT interact with the transactional write database (IUnitOfWork).
/// </summary>
public class GetShopByIdQueryHandler : IRequestHandler<GetShopByIdQuery, ShopDto?>
{
    private readonly ICacheService _cacheService;
    private readonly ITenantContext _tenantContext;

    public GetShopByIdQueryHandler(ICacheService cacheService, ITenantContext tenantContext)
    {
        _cacheService = cacheService;
        _tenantContext = tenantContext;
    }

    public async Task<ShopDto?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Authorization: Ensure a tenant is authenticated and authorized for this resource.
        var authenticatedTenantId = _tenantContext.TenantId;
        if (authenticatedTenantId is null)
        {
            throw new UnauthorizedAccessException();
        }
        if (authenticatedTenantId.Value != request.ShopId)
        {
            throw new InvalidOperationException("ForbiddenAccess");
        }

        // 2. Data Retrieval (Read-Side Path): Attempt to fetch the read model from the cache.
        string cacheKey = $"shop:{request.ShopId}";
        var shopReadModel = await _cacheService.GetAsync<ShopReadModel>(cacheKey);

        // 3. Cache Hit: If the read model is found, map it to the API DTO.
        if (shopReadModel is not null)
        {
            return new ShopDto
            {
                Id = shopReadModel.Id,
                Name = shopReadModel.Name,
                // Assuming we add these to the read model in the future
                // CreatedAt = shopReadModel.CreatedAt,
                DefaultLanguageCode = shopReadModel.DefaultLanguageCode
            };
        }

        // 4. Cache Miss: The data does not exist in our fast read store.
        // In a full production system, this might trigger a fallback to a read-replica database.
        // For our current design, a cache miss means the data is not available on the fast path.
        // The controller will interpret 'null' as a 404 Not Found.
        return null;
    }
}