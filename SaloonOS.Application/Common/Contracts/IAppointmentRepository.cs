using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Common.Contracts;

public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Gets all appointments for a given staff member on a specific day.
    /// </summary>
    Task<List<Appointment>> GetAppointmentsForDay(Guid shopId, Guid staffId, DateTime date);

    /// <summary>
    /// Checks if a proposed time slot is available, considering existing appointments.
    /// This is a critical method for preventing double-bookings.
    /// </summary>
    Task<bool> IsSlotAvailable(Guid shopId, Guid staffId, DateTime proposedStartTime, int durationInMinutes);
}