using SaloonOS.Application.Common.Contracts;
using SaloonOS.Infrastructure.Persistence.DbContext;
using SaloonOS.Infrastructure.Persistence.Repositories;

namespace SaloonOS.Infrastructure.Persistence;

/// <summary>
/// Implements the Unit of Work pattern to manage database transactions and repositories.
/// This class ensures that all operations within a single business transaction are either
/// all committed or all rolled back, maintaining data integrity. It acts as a facade
/// over the DbContext and exposes all repositories used by the application.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SaloonOSDbContext _context;
    private bool _disposed = false;

    // Repositories are lazily instantiated to save resources.
    private IShopRepository? _shops;

    public UnitOfWork(SaloonOSDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public IShopRepository Shops => _shops ??= new ShopRepository(_context);
    // When you add new repositories, expose them here:
    // public IAppointmentRepository Appointments => _appointments ??= new AppointmentRepository(_context);


    /// <inheritdoc />
    public async Task<int> CompleteAsync()
    {
        // This single call to SaveChangesAsync commits all the changes tracked by the DbContext
        // as a single transaction. This is the core of the Unit of Work pattern.
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose the DbContext, which closes the database connection.
                _context.Dispose();
            }
        }
        _disposed = true;
    }
}