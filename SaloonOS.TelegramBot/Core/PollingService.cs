// Path: Core/PollingService.cs
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling; // <-- This namespace is crucial for the modern polling receiver.
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SaloonOS.TelegramBot.Core;

/// <summary>
/// The main background service that continuously polls the Telegram API for updates.
/// This version uses the recommended 'ReceiveAsync' pattern with delegate handlers.
/// </summary>
public class PollingService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PollingService> _logger;
    private CancellationTokenSource? _cancellationTokenSource;

    public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting bot polling service...");
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // We resolve the bot client once at the start.
        var botClient = _serviceProvider.GetRequiredService<ITelegramBotClient>();

        var me = await botClient.GetMe(cancellationToken);
        _logger.LogInformation("Bot started successfully! ID: {BotId}, Name: {BotName}", me.Id, me.Username);

        var receiverOptions = new ReceiverOptions
        {
            // We specify which update types we want to receive.
            // Empty array means all except ChatMember related updates.
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        // StartReceiving does not block the thread. It will run in the background.
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler : HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _cancellationTokenSource.Token
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping bot polling service...");
        _cancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }

    /// <summary>
    /// This is the delegate method that Hangfire will call for each incoming update.
    /// </summary>
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // For each update, we create a new DI scope to resolve our scoped services.
        // This is the CRITICAL part for ensuring services like ApiClient are handled correctly.
        // The 'using' block ensures the scope is disposed of properly.
        using var scope = _serviceProvider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<UpdateDispatcher>();

        try
        {
            await dispatcher.DispatchAsync(update);
        }
        catch (Exception ex)
        {
            // Catch exceptions from the dispatcher so one failed update doesn't stop the polling.
            _logger.LogError(ex, "An error occurred while dispatching update {UpdateId}", update.Id);
        }
    }

    /// <summary>
    /// This is the delegate method that Hangfire will call if a polling error occurs.
    /// </summary>
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
}