// Path: Core/UpdateDispatcher.cs
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using SaloonOS.TelegramBot.Features.Onboarding;

namespace SaloonOS.TelegramBot.Core;

/// <summary>
/// The central router for the bot. Its single responsibility is to inspect an incoming
/// update from Telegram and dispatch it to the appropriate feature handler.
/// This promotes the Single Responsibility Principle and keeps feature logic decoupled.
/// </summary>
public class UpdateDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UpdateDispatcher> _logger;

    public UpdateDispatcher(IServiceProvider serviceProvider, ILogger<UpdateDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(Update update)
    {
        _logger.LogDebug("Dispatching update of type {UpdateType}", update.Type);

        IUpdateHandler? handler = update.Type switch
        {
            UpdateType.Message when update.Message!.Text!.StartsWith("/start") => GetHandler<StartCommandHandler>(),
            // Add more routes here as features are built
            // UpdateType.CallbackQuery when update.CallbackQuery!.Data!.StartsWith("main_") => GetHandler<MainMenuHandler>(),
            _ => null
        };

        if (handler is not null)
        {
            try
            {
                await handler.Handle(update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred in handler {HandlerType}", handler.GetType().Name);
                // Optionally, send a generic error message to the user here.
            }
        }
        else
        {
            _logger.LogWarning("No handler found for update type {UpdateType} with data: {Data}", update.Type, update.Message?.Text ?? update.CallbackQuery?.Data);
        }
    }

    private T GetHandler<T>() where T : notnull, IUpdateHandler => _serviceProvider.GetRequiredService<T>();
}

/// <summary>
/// A base interface that all feature handlers must implement.
/// </summary>
public interface IUpdateHandler
{
    Task Handle(Update update);
}