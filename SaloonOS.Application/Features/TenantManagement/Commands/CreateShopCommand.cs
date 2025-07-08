using MediatR;

namespace SaloonOS.Application.Features.TenantManagement.Commands;

public record CreateShopCommand(
    string Name,
    string ApiKey,
    string DefaultLanguageCode,
    Guid BusinessCategoryId // <-- ADDED
) : IRequest<Guid>;