// Path: SaloonOS.Application/Features/Notifications/AppointmentReminderJob.cs
using Microsoft.Extensions.Logging;
using SaloonOS.Application.Common.Contracts;
using System.Globalization;
// We'll need a way to get localized strings outside of an HTTP context.
// A dedicated service or a more complex IStringLocalizer setup is required.
// For now, we'll assume a simplified mechanism.

namespace SaloonOS.Application.Features.Notifications;

public class AppointmentReminderJob : IAppointmentReminderJob
{
    private readonly ILogger<AppointmentReminderJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public AppointmentReminderJob(
        ILogger<AppointmentReminderJob> logger,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task SendReminder(Guid appointmentId)
    {
        _logger.LogInformation("Executing reminder job for appointment ID: {AppointmentId}", appointmentId);

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
        if (appointment is null || appointment.Status != Domain.Booking.Entities.AppointmentStatus.Scheduled)
        {
            _logger.LogWarning("Skipping reminder for appointment {AppointmentId} as it was not found or is no longer scheduled.", appointmentId);
            return;
        }

        // Fetch all related entities needed to build the message.
        // In a real system, you'd fetch Customer, Service, StaffMember, and the Shop to get the language code.
        // var customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(appointment.CustomerId);
        // var service = await _unitOfWork.Services.GetByIdAsync(appointment.ServiceId);
        // var staff = await _unitOfWork.StaffMembers.GetByIdAsync(appointment.StaffMemberId);

        // Placeholder data
        var serviceName = "Haircut";
        var staffName = "John";
        var appointmentTime = appointment.StartTime.ToLocalTime().ToString("t"); // Format to local time
        var customerTelegramId = 12345L; // This would come from the Customer entity.

        // TODO: Properly resolve localized string for the customer's preferred language.
        string message = $"🔔 Reminder: Your appointment for {serviceName} with {staffName} is tomorrow at {appointmentTime}. We look forward to seeing you!";

        await _notificationService.SendMessageAsync(customerTelegramId, message, CancellationToken.None);

        _logger.LogInformation("Successfully sent reminder for appointment ID: {AppointmentId}", appointmentId);
    }
}