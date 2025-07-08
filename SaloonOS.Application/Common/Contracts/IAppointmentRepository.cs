using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Common.Contracts;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<List<Appointment>> GetAppointmentsForDay(Guid shopId, Guid staffId, DateTime date);
    Task<bool> IsSlotAvailable(Guid shopId, Guid staffId, DateTime proposedStartTime, int durationInMinutes);
    Task<List<Appointment>> GetUpcomingAppointmentsForCustomerAsync(Guid shopId, Guid customerId);
}