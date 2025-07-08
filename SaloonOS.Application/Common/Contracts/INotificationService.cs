// Path: SaloonOS.Application/Common/Contracts/INotificationService.cs
namespace SaloonOS.Application.Common.Contracts;

/// <summary>
/// Defines a contract for a service that sends notifications to users.
/// This abstraction allows us to easily swap out the delivery mechanism (Telegram, Email, SMS)
/// without changing the core application logic that triggers the notification.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a message to a specific user.
    /// </summary>
    /// <param name="recipientId">The unique identifier for the recipient (e.g., Telegram User ID).</param>
    /// <param name="message">The content of the message to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SendMessageAsync(long recipientId, string message, CancellationToken cancellationToken);
}