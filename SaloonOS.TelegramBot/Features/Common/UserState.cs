// Path: Features/Common/UserState.cs
namespace SaloonOS.TelegramBot.Features.Common;

/// <summary>
/// Represents the conversational state for a single user.
/// Stored by the StateService.
/// </summary>
public class UserState
{
    public ConversationStep CurrentStep { get; set; } = ConversationStep.Idle;
    public Guid CustomerId { get; set; }
    public string PreferredLanguageCode { get; set; } = "en-US";

    // Data collected during a booking flow
    public Guid? SelectedServiceId { get; set; }
    public Guid? SelectedStaffId { get; set; }
    public DateTime? SelectedDate { get; set; }
    public DateTime? SelectedTimeSlot { get; set; }
}

public enum ConversationStep
{
    Idle,
    AwaitingServiceSelection,
    AwaitingStaffSelection,
    AwaitingDaySelection,
    AwaitingTimeSlotSelection,
    AwaitingConfirmation
}