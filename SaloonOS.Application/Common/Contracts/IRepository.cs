using System.Linq.Expressions;

namespace SaloonOS.Application.Common.Contracts;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Remove(T entity);
    IQueryable<T> FindBy(Expression<Func<T, bool>> expression);
}