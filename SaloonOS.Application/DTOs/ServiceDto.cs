// Path: SaloonOS.Application/DTOs/ServiceDto.cs
namespace SaloonOS.Application.DTOs;

public class ServiceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string CurrencyCode { get; init; } = string.Empty; // Keep the code
    public string CurrencySymbol { get; init; } = string.Empty; // <-- ADD THIS FOR UI
    public int DurationInMinutes { get; init; }
}