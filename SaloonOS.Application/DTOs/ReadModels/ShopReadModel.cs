namespace SaloonOS.Application.DTOs.ReadModels;

/// <summary>
/// A denormalized, read-optimized representation of a Shop.
/// This model is designed to be stored in a fast read store like Redis
/// and contains all the data needed for a specific query, pre-joined and ready to serve.
/// </summary>
public class ShopReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string BusinessCategoryName { get; init; } = string.Empty;
    public string DefaultLanguageCode { get; init; } = string.Empty;
}