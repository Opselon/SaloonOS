// Path: Features/Onboarding/StartCommandHandler.cs
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using SaloonOS.TelegramBot.Core;
using SaloonOS.TelegramBot.Core.Services;

namespace SaloonOS.TelegramBot.Features.Onboarding;

/// <summary>
/// Handles the initial `/start` command from a user.
/// Its responsibility is to onboard the user by calling the API to get/create their
/// customer record and then present the main menu.
/// </summary>
public class StartCommandHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly StateService _stateService;
    private readonly ApiClient _apiClient;
    private readonly ILogger<StartCommandHandler> _logger;

    public StartCommandHandler(
        ITelegramBotClient botClient,
        StateService stateService,
        ApiClient apiClient,
        ILogger<StartCommandHandler> logger)
    {
        _botClient = botClient;
        _stateService = stateService;
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task Handle(Update update)
    {
        var message = update.Message!;
        var user = message.From!;
        _logger.LogInformation("Handling /start command for user {UserId}", user.Id);

        // 1. Get or Create the user via our powerful API command
        var customerCommand = new
        {
            TelegramUserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DetectedLanguageCode = user.LanguageCode ?? "en-US"
        };
        var customer = await _apiClient.GetOrCreateCustomer(customerCommand);

        if (customer is null)
        {
            await _botClient.SendMessage(message.Chat.Id, "Sorry, there was a problem setting up your profile. Please try again later.");
            return;
        }

        // 2. Update the local user state
        var userState = _stateService.GetOrCreateState(user.Id);
        userState.CustomerId = customer.Id;
        userState.PreferredLanguageCode = customer.PreferredLanguageCode;
        userState.CurrentStep = Common.ConversationStep.Idle;

        // 3. Build the main menu keyboard (this should come from a dedicated menu handler in the future)
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("📅 Book Appointment", "main_book") },
            new[] { InlineKeyboardButton.WithCallbackData("🗓️ My Appointments", "main_my_appointments") },
        });

        // 4. Send the welcome message (TODO: Use LocalizationService)
        string welcomeMessage = $"Welcome, {customer.FirstName}! How can I help?";
        await _botClient.SendMessage(
            chatId: message.Chat.Id,
            text: welcomeMessage,
            replyMarkup: keyboard);
    }
}