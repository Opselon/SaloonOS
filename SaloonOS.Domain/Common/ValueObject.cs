// Path: SaloonOS.Domain/Common/ValueObject.cs
namespace SaloonOS.Domain.Common;

/// <summary>
/// A base class for creating Value Objects, a core pattern in Domain-Driven Design.
/// Value Objects are defined by their attributes rather than a unique ID. Two Value Objects
/// are considered equal if all their constituent parts are equal. This class provides
/// the necessary equality comparison logic.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// When overridden in a derived class, this method returns all components
    /// of the value object that are used for equality comparison.
    /// </summary>
    /// <returns>An IEnumerable of the value object's components.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b)
    {
        return !(a == b);
    }
}