// Path: SaloonOS.Application/Features/Booking/Commands/BookAppointmentCommandHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;
using Hangfire;

namespace SaloonOS.Application.Features.Booking.Commands;

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IBackgroundJobClient _jobClient;

    public BookAppointmentCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IBackgroundJobClient jobClient)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _jobClient = jobClient;
    }

    public async Task<AppointmentDto> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException(nameof(Service), request.ServiceId);

        if (service.ShopId != shopId) throw new NotFoundException(nameof(Service), request.ServiceId);

        bool isSlotAvailable = await _unitOfWork.Appointments.IsSlotAvailable(
            shopId, request.StaffId, request.StartTime, service.DurationInMinutes);

        if (!isSlotAvailable)
        {
            throw new InvalidOperationException("TimeSlotNotAvailable");
        }

        // --- LOGIC CORRECTION ---
        // The 'appointment' variable MUST be created before it can be used.

        // 1. Create the appointment
        var appointment = Appointment.Create(
            shopId,
            request.CustomerId,
            request.ServiceId,
            request.StaffId,
            request.StartTime,
            service.DurationInMinutes,
            service.Price,
            service.Currency);

        await _unitOfWork.Appointments.AddAsync(appointment);
        await _unitOfWork.CompleteAsync();

        // 2. NOW schedule the job using the created appointment object.
        var reminderTime = appointment.StartTime.AddHours(-24);
        if (reminderTime > DateTime.UtcNow)
        {
            _jobClient.Schedule<IAppointmentReminderJob>(
                job => job.SendReminder(appointment.Id),
                reminderTime);
        }

        // TODO: Publish AppointmentBookedEvent

        return new AppointmentDto
        {
            Id = appointment.Id,
            StartTime = appointment.StartTime,
            Status = appointment.Status.ToString(),
            CustomerId = appointment.CustomerId,
            ServiceId = appointment.ServiceId,
            StaffId = appointment.StaffMemberId
        };
    }
}