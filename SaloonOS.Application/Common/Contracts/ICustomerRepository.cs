// Path: SaloonOS.Application/Common/Contracts/ICustomerRepository.cs
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Common.Contracts;

public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Finds a customer within a specific shop by their Telegram User ID.
    /// </summary>
    /// <returns>The Customer entity or null if not found.</returns>
    Task<Customer?> GetByTelegramIdAsync(Guid shopId, long telegramUserId);
}