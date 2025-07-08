using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Common.Contracts;

// This interface currently serves as a marker. As we add staff-specific
// query needs, we will add method signatures here.
public interface IStaffMemberRepository : IRepository<StaffMember>
{
    // Example for the future:
    // Task<IEnumerable<StaffMember>> GetStaffAvailableForService(Guid serviceId);
}