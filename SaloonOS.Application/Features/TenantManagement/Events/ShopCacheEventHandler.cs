using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs.ReadModels;
using SaloonOS.Domain.TenantManagement.Events;

namespace SaloonOS.Application.Features.TenantManagement.Events;

/// <summary>
/// This handler listens for the ShopCreatedEvent. Its SOLE responsibility is to
/// create and cache the denormalized read model for the new shop.
/// This runs asynchronously and does not block the original API request.
/// </summary>
public class ShopCacheEventHandler : INotificationHandler<ShopCreatedEvent>
{
    private readonly ICacheService _cacheService;

    public ShopCacheEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ShopCreatedEvent notification, CancellationToken cancellationToken)
    {
        var shop = notification.Shop;
        var readModel = new ShopReadModel
        {
            Id = shop.Id,
            Name = shop.Name,
            // In a real scenario, you'd fetch the category name.
            BusinessCategoryName = "Placeholder Category",
            DefaultLanguageCode = shop.DefaultLanguageCode
        };

        // Cache the read model with a key that is easy to look up.
        string cacheKey = $"shop:{shop.Id}";
        await _cacheService.SetAsync(cacheKey, readModel);
    }
}