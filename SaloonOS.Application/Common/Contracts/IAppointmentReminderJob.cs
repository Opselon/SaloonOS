// Path: SaloonOS.Application/Common/Contracts/IAppointmentReminderJob.cs
namespace SaloonOS.Application.Common.Contracts;

/// <summary>
/// Defines the contract for our background job.
/// Hangfire works best with interfaces for dependency injection and testing.
/// The method defined here is what Hangfire will actually execute.
/// </summary>
public interface IAppointmentReminderJob
{
    /// <summary>
    /// The public method that will be triggered by Hangfire.
    /// It MUST be public.
    /// </summary>
    /// <param name="appointmentId">The ID of the appointment to send a reminder for.</param>
    Task SendReminder(Guid appointmentId);
}