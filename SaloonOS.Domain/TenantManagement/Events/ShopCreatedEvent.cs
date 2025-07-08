using MediatR;
using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Domain.TenantManagement.Events;

// A MediatR INotification that will be published when a shop is created.
public record ShopCreatedEvent(Shop Shop) : INotification;