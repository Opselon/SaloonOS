using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.DTOs.ReadModels;

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

        // 2. Cache Miss / Fallback Path: Data is not in Redis. We must build the read model from the main database.
        // This operation is slower but necessary to populate the cache for subsequent requests.
        var servicesFromDb = await _unitOfWork.Services.ListByShopIdAsync(tenantId, request.LanguageCode); // Assuming this method eagerly loads all translations

        if (!servicesFromDb.Any())
        {
            return Enumerable.Empty<ServiceDto>();
        }

        // 3. Build & Cache the Read Model
        var servicesToCache = servicesFromDb.Select(s => new ServiceReadModel
        {
            Id = s.Id,
            Price = s.Price,
            Currency = s.Currency,
            DurationInMinutes = s.DurationInMinutes,
            Translations = s.Translations.ToDictionary(
                t => t.LanguageCode,
                t => new ServiceTranslationModel { Name = t.Name, Description = t.Description })
        }).ToList();

        // Cache the result for future requests. Use a reasonable expiry.
        await _cacheService.SetAsync(cacheKey, servicesToCache, TimeSpan.FromHours(1));

        // 4. Map and return the result for this initial request.
        return MapToDto(servicesToCache, request.LanguageCode);
    }

    private IEnumerable<ServiceDto> MapToDto(List<ServiceReadModel> readModels, string languageCode)
    {
        return readModels.Select(rm =>
        {
            // Attempt to get the requested language; fall back to the first available if not found.
            var hasTranslation = rm.Translations.TryGetValue(languageCode, out var translation);
            if (!hasTranslation)
            {
                translation = rm.Translations.Values.FirstOrDefault();
            }

            return new ServiceDto
            {
                Id = rm.Id,
                Name = translation?.Name ?? "No Translation",
                Description = translation?.Description,
                Price = rm.Price,
                Currency = rm.Currency,
                DurationInMinutes = rm.DurationInMinutes
            };
        });
    }
}