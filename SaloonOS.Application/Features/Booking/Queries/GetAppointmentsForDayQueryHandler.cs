// Path: SaloonOS.Application/Features/Booking/Queries/GetAppointmentsForDayQueryHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using System.Linq;

namespace SaloonOS.Application.Features.Booking.Queries;

public class GetAppointmentsForDayQueryHandler : IRequestHandler<GetAppointmentsForDayQuery, IEnumerable<AdminAppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetAppointmentsForDayQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<AdminAppointmentDto>> Handle(GetAppointmentsForDayQuery request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        // --- AUTHORIZATION ---
        // Verify that the user making the request is the owner of the shop.
        var shop = await _unitOfWork.Shops.GetByIdAsync(shopId)
            ?? throw new InvalidOperationException("Shop context not found.");

        if (shop.OwnerTelegramUserId != request.AdminTelegramUserId)
        {
            throw new UnauthorizedAccessException("AdminNotAuthorized");
        }

        // --- DATA RETRIEVAL ---
        var appointments = await _unitOfWork.Appointments.GetScheduledAppointmentsForShopByDayAsync(shopId, request.Date);

        // In a real high-performance system, this mapping step would be more complex.
        // You would fetch all referenced Customers, Services, and Staff in batches to avoid N+1 query problems.
        // For example:
        // var serviceIds = appointments.Select(a => a.ServiceId).ToList();
        // var services = await _unitOfWork.Services.GetByIdsAsync(serviceIds);
        // Then, you would build a lookup dictionary for efficient mapping.

        // For now, we'll use a placeholder mapping.
        return appointments.Select(a => new AdminAppointmentDto
        {
            Id = a.Id,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = a.Status.ToString(),
            Price = a.Price,
            // These would be populated from the batched lookups mentioned above.
            CustomerName = $"Customer-{a.CustomerId.ToString().Substring(0, 4)}",
            ServiceName = $"Service-{a.ServiceId.ToString().Substring(0, 4)}",
            StaffName = $"Staff-{a.StaffMemberId.ToString().Substring(0, 4)}"
        });
    }
}