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
    private readonly Dictionary<Type, object> _repositories = new();
    // Repositories are lazily instantiated to save resources.
    private IShopRepository? _shops;
    private IServiceRepository? _services;
    private IStaffMemberRepository? _staffMembers; // Corrected: private backing field inside class scope
    private IAppointmentRepository? _appointments;

    public UnitOfWork(SaloonOSDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public IShopRepository Shops => _shops ??= new ShopRepository(_context);
    public IServiceRepository Services => _services ??= new ServiceRepository(_context);
    public IStaffMemberRepository StaffMembers => _staffMembers ??= new StaffMemberRepository(_context);

    public IAppointmentRepository Appointments => _appointments ??= new AppointmentRepository(_context);

    /// <inheritdoc />
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public IRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
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
                _context.Dispose();
            }
        }
        _disposed = true;
    }
}