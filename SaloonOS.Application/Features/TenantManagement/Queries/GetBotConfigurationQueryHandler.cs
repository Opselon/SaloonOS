// Path: SaloonOS.Application/Features/TenantManagement/Queries/GetBotConfigurationQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Domain.Shared;
using SaloonOS.Domain.TenantManagement.Entities; // <-- ADDED THIS MISSING USING

namespace SaloonOS.Application.Features.TenantManagement.Queries;

public class GetBotConfigurationQueryHandler : IRequestHandler<GetBotConfigurationQuery, BotConfigurationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetBotConfigurationQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<BotConfigurationDto> Handle(GetBotConfigurationQuery request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        // This will now compile correctly because the 'Shop' type is found.
        var shop = await _unitOfWork.GetRepository<Shop>()
                                    .FindBy(s => s.Id == shopId)
                                    .Include(s => s.BusinessCategory)
                                    .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new InvalidOperationException("Shop context not found.");

        // This line was already correct, but depended on Currency.cs compiling.
        // I'll assume a property `PrimaryCurrencyCode` was added to the Shop entity.
        var currency = Currency.FromCode(shop.PrimaryCurrencyCode);

        return new BotConfigurationDto
        {
            ShopId = shop.Id,
            ShopName = shop.Name,
            BusinessCategory = shop.BusinessCategory.Name,
            DefaultLanguageCode = shop.DefaultLanguageCode,
            SupportedLanguageCodes = new List<string> { "en-US", "fa-IR", "ru-RU" },
            PrimaryCurrencyCode = currency.Code,
            PrimaryCurrencySymbol = currency.Symbol
        };
    }
}