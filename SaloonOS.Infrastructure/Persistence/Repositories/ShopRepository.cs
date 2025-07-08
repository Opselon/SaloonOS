using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.TenantManagement.Entities;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

/// <summary>
/// A specific repository for the Shop entity. It inherits from the generic Repository
/// and implements the IShopRepository interface, providing any custom data access
/// methods required for Shops that are not covered by the generic implementation.
/// </summary>
public class ShopRepository : Repository<Shop>, IShopRepository
{
    public ShopRepository(SaloonOSDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Shop?> GetByHashedApiKeyAsync(string hashedApiKey)
    {
        // This custom method is crucial for our API key authentication middleware.
        // It provides an efficient way to find a tenant by their unique API key.
        return await _context.Shops
            .FirstOrDefaultAsync(s => s.HashedApiKey == hashedApiKey);
    }
}