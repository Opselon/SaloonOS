using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Queries;

/// <summary>
/// Represents a query to retrieve a list of all active services for the authenticated shop.
/// The language code is used to select the appropriate translation from the read model.
/// </summary>
public record ListServicesByShopQuery(string LanguageCode) : IRequest<IEnumerable<ServiceDto>>;