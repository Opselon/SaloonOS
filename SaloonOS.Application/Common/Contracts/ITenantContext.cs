namespace SaloonOS.Application.Common.Contracts;

/// <summary>
/// Defines a contract for a service that provides information about the current tenant.
/// This abstraction allows the application layer to remain ignorant of how the tenant
/// information is retrieved (e.g., from an HTTP context, a message queue header, etc.),
/// promoting loose coupling and testability.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the unique identifier of the current authenticated tenant (Shop).
    /// Returns null if no tenant is authenticated.
    /// </summary>
    Guid? TenantId { get; }
}