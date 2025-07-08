// Path: SaloonOS.Application/Features/Booking/Queries/ListServicesByShopQueryHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.DTOs.ReadModels;
using SaloonOS.Domain.Shared; // For the Currency Value Object
using System.Linq;

namespace SaloonOS.Application.Features.Booking.Queries;

public class ListServicesByShopQueryHandler : IRequestHandler<ListServicesByShopQuery, IEnumerable<ServiceDto>>
{
    private readonly ICacheService _cacheService;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork; // Fallback to write DB

    public ListServicesByShopQueryHandler(ICacheService cacheService, ITenantContext tenantContext, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ServiceDto>> Handle(ListServicesByShopQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();
        string cacheKey = $"services:shop:{tenantId}";

        // 1. Primary Path: Attempt to get the entire list from Redis.
        var cachedServices = await _cacheService.GetAsync<List<ServiceReadModel>>(cacheKey);
        if (cachedServices is not null)
        {
            return MapToDto(cachedServices, request.LanguageCode);
        }

        // 2. Cache Miss / Fallback Path: Data is not in Redis.
        var servicesFromDb = await _unitOfWork.Services.ListByShopIdAsync(tenantId, request.LanguageCode);

        if (!servicesFromDb.Any())
        {
            return Enumerable.Empty<ServiceDto>();
        }

        // 3. Build & Cache the Read Model
        var servicesToCache = servicesFromDb.Select(s => new ServiceReadModel
        {
            Id = s.Id,
            Price = s.Price,
            // --- CORRECTED ---
            // The domain entity stores the currency code in a property named 'Currency'.
            // We map this to the 'CurrencyCode' property in our read model.
            CurrencyCode = s.Currency,
            // ---
            DurationInMinutes = s.DurationInMinutes,
            Translations = s.Translations.ToDictionary(
                t => t.LanguageCode,
                t => new ServiceTranslationModel { Name = t.Name, Description = t.Description })
        }).ToList();

        await _cacheService.SetAsync(cacheKey, servicesToCache, TimeSpan.FromHours(1));

        // 4. Map and return the result for this initial request.
        return MapToDto(servicesToCache, request.LanguageCode);
    }

    private IEnumerable<ServiceDto> MapToDto(List<ServiceReadModel> readModels, string languageCode)
    {
        return readModels.Select(rm =>
        {
            // ... existing translation logic ...
            if (!rm.Translations.TryGetValue(languageCode, out var translation))
            {
                translation = rm.Translations.Values.FirstOrDefault();
            }

            // Use our domain's Currency Value Object to get the symbol from the code.
            var currency = Currency.FromCode(rm.CurrencyCode);

            // --- CORRECTED ---
            // Map to the final DTO using the correct property names: 'CurrencyCode' and 'CurrencySymbol'.
            return new ServiceDto
            {
                Id = rm.Id,
                Name = translation?.Name ?? "No Translation",
                Description = translation?.Description,
                Price = rm.Price,
                CurrencyCode = currency.Code,
                CurrencySymbol = currency.Symbol,
                DurationInMinutes = rm.DurationInMinutes
            };
            // ---
        });
    }
}