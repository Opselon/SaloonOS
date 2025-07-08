using System.Security.Cryptography;
using System.Text;
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Application.Features.TenantManagement.Commands;

/// <summary>
/// Handles the business logic for the CreateShopCommand.
/// This class is responsible for taking the command data, performing the necessary operations
/// (like hashing the API key), creating the domain entity, and persisting it to the database.
/// </summary>
public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateShopCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        // 1. Business Logic: Hash the incoming API key. NEVER store raw keys.
        var hashedApiKey = HashApiKey(request.ApiKey);

        // In a real application, you would check if a shop with this name or API key already exists.
        // var existingShop = await _unitOfWork.Shops.GetByHashedApiKeyAsync(hashedApiKey);
        // if (existingShop != null) throw new SomeCustomException("API Key is already in use.");

        // 2. Domain Logic: Create the domain entity using our factory method.
        var shop = Shop.Create(
            request.Name,
            hashedApiKey,
            request.DefaultLanguageCode
        );

        // 3. Persistence: Add the new entity to the repository.
        await _unitOfWork.Shops.AddAsync(shop);

        // 4. Transaction: Commit all changes to the database as a single atomic operation.
        await _unitOfWork.CompleteAsync();

        // 5. Response: Return the ID of the newly created entity.
        return shop.Id;
    }

    /// <summary>
    /// Hashes a given string using SHA256. This is a one-way operation.
    /// </summary>
    private static string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(apiKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}