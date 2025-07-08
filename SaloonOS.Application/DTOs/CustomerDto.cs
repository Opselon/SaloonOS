// Path: SaloonOS.Application/DTOs/CustomerDto.cs
namespace SaloonOS.Application.DTOs;

/// <summary>
/// A DTO representing the public data of a Customer.
/// </summary>
public class CustomerDto
{
    public Guid Id { get; init; }
    public long TelegramUserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string PreferredLanguageCode { get; init; } = string.Empty;
}