namespace SaloonOS.Application.DTOs.ReadModels;

/// <summary>
/// A denormalized, read-optimized representation of a Service with its translations.
/// This model is designed to be stored as a collection in Redis under a key
/// specific to the shop, allowing for extremely fast retrieval of all services for a tenant.
/// </summary>
// Path: SaloonOS.Application/DTOs/ReadModels/ServiceReadModel.cs
public class ServiceReadModel
{
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public string CurrencyCode { get; init; } = string.Empty; // <-- ADD THIS
    public int DurationInMinutes { get; init; }
    public Dictionary<string, ServiceTranslationModel> Translations { get; init; } = new();
}

public class ServiceTranslationModel
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}