using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(SaloonOSDbContext context) : base(context) { }

    public async Task<IEnumerable<Service>> ListByShopIdAsync(Guid shopId, string languageCode)
    {
        // This is a powerful EF Core query. It filters services by the tenant's shopId,
        // then for each service, it loads ONLY the specific translation that matches the requested language.
        // This is highly efficient as it avoids loading all translations for all services.
        return await _context.Services
            .Where(s => s.ShopId == shopId && s.IsActive)
            .Include(s => s.Translations.Where(t => t.LanguageCode == languageCode))
            .ToListAsync();
    }
}