namespace SaloonOS.Application.DTOs;

/// <summary>
/// A Data Transfer Object for the Shop entity. This is a crucial pattern
/// that defines the public "shape" of our Shop data. It decouples the API contract
/// from the internal domain model, preventing breaking changes in the API when the
/// domain model evolves and protecting against accidental exposure of sensitive data.
/// </summary>
public class ShopDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DefaultLanguageCode { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}