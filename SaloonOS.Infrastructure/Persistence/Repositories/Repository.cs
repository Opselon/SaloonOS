using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Infrastructure.Persistence.DbContext;
using System.Linq.Expressions;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

/// <summary>
/// A generic repository implementation that provides base data access functionalities for any entity.
/// This reduces boilerplate code for standard CRUD operations.
/// </summary>
/// <typeparam name="T">The type of the entity. Must be a class.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SaloonOSDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(SaloonOSDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <inheritdoc />
    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc />
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Where(expression);
    }
}