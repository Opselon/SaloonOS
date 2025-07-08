// Path: SaloonOS.Application/Features/TenantManagement/Commands/CreateShopCommand.cs
using MediatR;

namespace SaloonOS.Application.Features.TenantManagement.Commands;

public record CreateShopCommand(
    string Name,
    string ApiKey,
    string DefaultLanguageCode,
    Guid BusinessCategoryId,
    string PrimaryCurrencyCode // <-- ADDED
) : IRequest<Guid>;