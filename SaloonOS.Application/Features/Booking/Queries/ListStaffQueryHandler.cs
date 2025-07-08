// Path: SaloonOS.Application/Features/Booking/Queries/ListStaffQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using System.Linq;

namespace SaloonOS.Application.Features.Booking.Queries;

public class ListStaffQueryHandler : IRequestHandler<ListStaffQuery, IEnumerable<StaffMemberDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public ListStaffQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<StaffMemberDto>> Handle(ListStaffQuery request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        var staffMembers = await _unitOfWork.StaffMembers.FindBy(s => s.ShopId == shopId && s.IsActive).ToListAsync();

        return staffMembers.Select(s => new StaffMemberDto
        {
            Id = s.Id,
            Name = s.Name
            // Add other properties as needed
        });
    }
}