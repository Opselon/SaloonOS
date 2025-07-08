namespace SaloonOS.Application.DTOs;

public class ServiceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int DurationInMinutes { get; init; }
}