using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Events; // <-- This namespace is now valid.

namespace SaloonOS.Application.Features.Booking.Events;

/// <summary>
/// This handler is responsible for cache invalidation for the services list.
/// When a service is created, updated, or deleted, we must remove the cached list
/// so that the next request rebuilds it with the fresh data from the database.
/// </summary>
public class ServiceCacheEventHandler :
    INotificationHandler<ServiceCreatedEvent>, // This type is now found.
    INotificationHandler<ServiceUpdatedEvent>, // This type is now found.
    INotificationHandler<ServiceDeletedEvent>  // This type is now found.
{
    private readonly ICacheService _cacheService;

    public ServiceCacheEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    // A single method to handle invalidation for any relevant event.
    private Task InvalidateCache(Guid shopId, CancellationToken cancellationToken)
    {
        string cacheKey = $"services:shop:{shopId}";
        return _cacheService.RemoveAsync(cacheKey);
    }

    public Task Handle(ServiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Service.ShopId, cancellationToken);
    }

    public Task Handle(ServiceUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Service.ShopId, cancellationToken);
    }

    public Task Handle(ServiceDeletedEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.ShopId, cancellationToken);
    }
}