namespace SaloonOS.Application.Common.Configuration;

/// <summary>
/// Holds configuration settings for the Stripe payment gateway.
/// These values will be bound from the application's configuration sources (e.g., appsettings.json).
/// </summary>
public class StripeSettings
{
    /// <summary>
    /// Your Stripe public key, used for client-side operations and some server-side validations.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Your Stripe secret key, used for all server-side API calls to Stripe.
    /// This should be kept highly confidential and managed securely.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
}