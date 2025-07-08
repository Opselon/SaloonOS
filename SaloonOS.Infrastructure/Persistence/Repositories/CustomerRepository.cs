// Path: SaloonOS.Infrastructure/Persistence/Repositories/CustomerRepository.cs
using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(SaloonOSDbContext context) : base(context) { }

    public async Task<Customer?> GetByTelegramIdAsync(Guid shopId, long telegramUserId)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.ShopId == shopId && c.TelegramUserId == telegramUserId);
    }
}