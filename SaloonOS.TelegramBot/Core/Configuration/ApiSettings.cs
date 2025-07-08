// Path: Core/Configuration/ApiSettings.cs
namespace SaloonOS.TelegramBot.Core.Configuration;

/// <summary>
/// Holds the configuration required to connect to the backend SaloonOS API.
/// Binds to the "ApiSettings" section of appsettings.json.
/// </summary>
public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}