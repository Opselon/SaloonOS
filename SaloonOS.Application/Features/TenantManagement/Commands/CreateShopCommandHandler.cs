// Path: SaloonOS.Application/Features/TenantManagement/Commands/CreateShopCommandHandler.cs

// --- REQUIRED USING DIRECTIVES ---
using MediatR;
using Microsoft.Extensions.Logging;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Shared; // For Currency Value Object
using SaloonOS.Domain.TenantManagement.Entities;
using SaloonOS.Domain.TenantManagement.Events;
using System.Security.Cryptography;
using System.Text;

// --- NAMESPACE DECLARATION ---
namespace SaloonOS.Application.Features.TenantManagement.Commands;

/// <summary>
/// Handles the business logic for creating a new Shop tenant.
/// </summary>
public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;
    private readonly ILogger<CreateShopCommandHandler> _logger;

    public CreateShopCommandHandler(
        IUnitOfWork unitOfWork,
        IPublisher publisher,
        ILogger<CreateShopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Business Category Existence
        var businessCategory = await _unitOfWork.GetRepository<BusinessCategory>().GetByIdAsync(request.BusinessCategoryId);
        if (businessCategory is null)
        {
            throw new ArgumentException($"Business category with ID '{request.BusinessCategoryId}' not found.", nameof(request.BusinessCategoryId));
        }

        // 2. Validate Currency Code
        try
        {
            Currency.FromCode(request.PrimaryCurrencyCode);
        }
        catch (NotSupportedException ex)
        {
            throw new ArgumentException("The provided currency code is not supported.", nameof(request.PrimaryCurrencyCode), ex);
        }

        // 3. Hash the API Key
        // This call will now resolve correctly because the method is defined below.
        var hashedApiKey = HashApiKey(request.ApiKey);

        // 4. Create the Domain Entity
        var shop = Shop.Create(
             request.Name,
             hashedApiKey,
             request.DefaultLanguageCode,
             request.BusinessCategoryId,
             request.PrimaryCurrencyCode
         );

        // 5. Persistence
        await _unitOfWork.Shops.AddAsync(shop);
        await _unitOfWork.CompleteAsync();

        // 6. Publish Event
        await _publisher.Publish(new ShopCreatedEvent(shop), cancellationToken);

        _logger.LogInformation("Shop '{ShopName}' created successfully with ID: {ShopId}", shop.Name, shop.Id);
        return shop.Id;
    }

    // --- HELPER METHOD IMPLEMENTATION ---
    // The missing method is now included as a private static helper within the class.

    /// <summary>
    /// Hashes a given string using SHA256. This is a one-way operation.
    /// This helper must be consistent with any other place API keys are hashed (e.g., authentication middleware).
    /// </summary>
    private static string HashApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), "API Key cannot be null or empty.");
        }

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(apiKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}