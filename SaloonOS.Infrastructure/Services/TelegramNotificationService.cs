// Path: SaloonOS.Infrastructure/Services/TelegramNotificationService.cs
using Microsoft.Extensions.Logging;
using SaloonOS.Application.Common.Contracts;
using Telegram.Bot;

namespace SaloonOS.Infrastructure.Services;

/// <summary>
/// Implements the notification service using the Telegram.Bot library.
/// This class is responsible for taking a message and a recipient ID and
/// using the Telegram API to send it.
/// </summary>
public class TelegramNotificationService : INotificationService
{
    private readonly ILogger<TelegramNotificationService> _logger;
    // Note: In a multi-bot architecture, you wouldn't have a single bot client here.
    // You would need a BotClientFactory that can retrieve the correct ITelegramBotClient
    // based on the ShopId associated with the recipient. This is a simplified version.
    // For now, we assume a single bot token is configured for the whole system.
    private readonly ITelegramBotClient _botClient;

    public TelegramNotificationService(ITelegramBotClient botClient, ILogger<TelegramNotificationService> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task SendMessageAsync(long recipientId, string message, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.SendMessage(
                chatId: recipientId,
                text: message,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Telegram message to user {RecipientId}", recipientId);
            // We do not re-throw here. A failed notification should not crash the background job.
        }
    }
}