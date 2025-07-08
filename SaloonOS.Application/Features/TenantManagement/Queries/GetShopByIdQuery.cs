using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.TenantManagement.Queries;

/// <summary>
/// Represents the query to retrieve details for a specific shop.
/// It carries the ID of the shop to be fetched and specifies that its
/// handler will return a ShopDto object.
/// </summary>
public record GetShopByIdQuery(Guid ShopId) : IRequest<ShopDto?>;