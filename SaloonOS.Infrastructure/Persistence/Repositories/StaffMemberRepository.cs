using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

// Standard repository implementation for StaffMember.
public class StaffMemberRepository : Repository<StaffMember>, IStaffMemberRepository
{
    public StaffMemberRepository(SaloonOSDbContext context) : base(context)
    {
    }
}