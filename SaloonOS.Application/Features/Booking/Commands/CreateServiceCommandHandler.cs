using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Domain.Booking.Events;

namespace SaloonOS.Application.Features.Booking.Commands;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IPublisher _publisher; // <-- Inject IPublisher

    public CreateServiceCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext, IPublisher publisher) // <-- Update constructor
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _publisher = publisher; // <-- Assign publisher
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        // 1. Authorization & Context: Ensure a valid tenant is making the request.
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException("Cannot create a service without an authenticated tenant.");
        var shop = await _unitOfWork.Shops.GetByIdAsync(tenantId) ?? throw new InvalidOperationException($"Shop not found for Tenant ID: {tenantId}");

        // 2. Domain Logic: Use the domain's factory method to create the Service aggregate.
        // This enforces all business rules and invariants at the point of creation.
        var service = Service.Create(
            shopId: shop.Id,
            price: request.Price,
            currency: request.Currency,
            durationInMinutes: request.DurationInMinutes,
            initialLanguageCode: shop.DefaultLanguageCode, // Use the shop's default language for the first translation.
            initialName: request.Name,
            initialDescription: request.Description
        );

        // 3. Persistence: Add the newly created aggregate to the repository.
        // EF Core's change tracker will automatically detect the related ServiceTranslation and add it as well.

        // 4. Transaction: Commit all changes to the database as a single, atomic operation.
        await _unitOfWork.Services.AddAsync(service);
        await _unitOfWork.CompleteAsync();
        await _publisher.Publish(new ServiceCreatedEvent(service), cancellationToken);
        // 5. Mapping & Response: Map the persisted entity to a DTO for the API response.
        // It's crucial to return the data from the object that was just saved, including its new ID.
        var translation = service.Translations.First();
        var currency = SaloonOS.Domain.Shared.Currency.FromCode(service.Currency); // Get the Value Object

        return new ServiceDto
        {
            Id = service.Id,
            Name = translation.Name,
            Description = translation.Description,
            Price = service.Price,
            // --- CORRECTED ---
            CurrencyCode = currency.Code,
            CurrencySymbol = currency.Symbol,
            // ---
            DurationInMinutes = service.DurationInMinutes
        };

    }

}