using MediatR;

namespace SaloonOS.Application.Features.TenantManagement.Commands;

/// <summary>
/// Represents the command to create a new shop tenant in the system.
/// This is a DTO that carries the necessary data from the API layer to the handler.
/// It implements IRequest from MediatR, specifying its response type (the Guid of the new shop).
/// </summary>
public record CreateShopCommand(
    string Name,
    string ApiKey, // We receive the raw key; the handler will hash it.
    string DefaultLanguageCode
) : IRequest<Guid>;