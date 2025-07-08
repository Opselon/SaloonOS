// Path: SaloonOS.Application/DTOs/BotConfigurationDto.cs
namespace SaloonOS.Application.DTOs;

/// <summary>
/// A comprehensive DTO that provides all the initial static configuration
/// a bot client needs to initialize itself for a specific shop.
/// </summary>
public class BotConfigurationDto
{
    public Guid ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public string BusinessCategory { get; init; } = string.Empty;
    public string DefaultLanguageCode { get; init; } = string.Empty;
    public List<string> SupportedLanguageCodes { get; init; } = new();
    public string PrimaryCurrencyCode { get; init; } = string.Empty;
    public string PrimaryCurrencySymbol { get; init; } = string.Empty;
}