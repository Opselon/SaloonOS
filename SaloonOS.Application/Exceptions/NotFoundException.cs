namespace SaloonOS.Application.Exceptions;

/// <summary>
/// A custom exception to be thrown when a specific entity is not found.
/// This allows for structured, predictable error handling in the API layer,
/// enabling a global exception handler to catch this specific type and return a 404 Not Found response.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" with key ({key}) was not found.")
    {
    }
}