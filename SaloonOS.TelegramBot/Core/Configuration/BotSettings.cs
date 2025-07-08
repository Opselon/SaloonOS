// Path: Core/Configuration/BotSettings.cs
namespace SaloonOS.TelegramBot.Core.Configuration;

/// <summary>
/// Holds the configuration specific to the Telegram Bot itself, like its access token.
/// Binds to the "BotSettings" section of appsettings.json.
/// </summary>
public class BotSettings
{
    public string Token { get; set; } = string.Empty;
}