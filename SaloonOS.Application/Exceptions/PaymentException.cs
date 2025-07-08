namespace SaloonOS.Application.Exceptions;

/// <summary>
/// A custom exception thrown for any issue related to payment processing.
/// This allows the API to catch payment-specific errors and return a structured
/// 400 Bad Request or 502 Bad Gateway response, rather than a generic 500.
/// </summary>
public class PaymentException : Exception
{
    public PaymentException(string message) : base(message)
    {
    }

    public PaymentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}