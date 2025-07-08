// Path: SaloonOS.Application/Exceptions/ConcurrencyException.cs

namespace SaloonOS.Application.Exceptions;

/// <summary>
/// A custom exception thrown when an optimistic concurrency check fails.
/// This typically occurs when a user tries to save an entity that has been modified
/// by another user since it was originally read. This exception can be caught by
/// a global error handler to return a 409 Conflict HTTP status code.
/// </summary>
public class ConcurrencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConcurrencyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}