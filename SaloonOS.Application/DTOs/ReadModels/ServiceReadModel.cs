namespace SaloonOS.Application.DTOs.ReadModels;

/// <summary>
/// A denormalized, read-optimized representation of a Service with its translations.
/// This model is designed to be stored as a collection in Redis under a key
/// specific to the shop, allowing for extremely fast retrieval of all services for a tenant.
/// </summary>
public class ServiceReadModel
{
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int DurationInMinutes { get; init; }

    // We store all translations directly in the read model to avoid joins on the read path.
    public Dictionary<string, ServiceTranslationModel> Translations { get; init; } = new();
}

public class ServiceTranslationModel
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}