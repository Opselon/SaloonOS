namespace SaloonOS.Domain.Common;

// A simple base class for all our entities to ensure they have a primary key.
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}