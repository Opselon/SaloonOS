using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Common.Contracts;

public interface IServiceRepository : IRepository<Service>
{
    /// <summary>
    /// Retrieves a list of services for a specific shop, eagerly loading the translation
    /// for the specified language.
    /// </summary>
    /// <param name="shopId">The ID of the shop.</param>
    /// <param name="languageCode">The language code for the desired translation (e.g., "en-US").</param>
    /// <returns>A list of services with their specified translations.</returns>
    Task<IEnumerable<Service>> ListByShopIdAsync(Guid shopId, string languageCode);
}