// Path: SaloonOS.Application/Features/Booking/Commands/GetOrCreateCustomerByTelegramIdCommandHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Features.Booking.Commands;

public class GetOrCreateCustomerByTelegramIdCommandHandler : IRequestHandler<GetOrCreateCustomerByTelegramIdCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetOrCreateCustomerByTelegramIdCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<CustomerDto> Handle(GetOrCreateCustomerByTelegramIdCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        // 1. Attempt to GET the customer.
        var customer = await _unitOfWork.Customers.GetByTelegramIdAsync(shopId, request.TelegramUserId);

        // 2. If the customer does not exist, CREATE them.
        if (customer is null)
        {
            customer = Customer.Create(
                shopId,
                request.TelegramUserId,
                request.FirstName,
                request.LastName,
                request.DetectedLanguageCode);

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();

            // TODO: Publish CustomerCreatedEvent
        }

        // 3. Map to DTO and return the result.
        return new CustomerDto
        {
            Id = customer.Id,
            TelegramUserId = customer.TelegramUserId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PreferredLanguageCode = customer.PreferredLanguageCode
        };
    }
}