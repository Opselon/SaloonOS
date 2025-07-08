namespace SaloonOS.Application.Common.Contracts;

// This will manage our transactions to ensure data consistency.
public interface IUnitOfWork : IDisposable
{
    IShopRepository Shops { get; }
    // Add other repositories here, e.g., IAppointmentRepository Appointments { get; }

    Task<int> CompleteAsync();
}