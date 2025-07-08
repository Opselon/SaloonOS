using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.TenantManagement.Entities;

public class Shop : BaseEntity
{
    public string Name { get; private set; }
    public string HashedApiKey { get; private set; }
    public string DefaultLanguageCode { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Private constructor for EF Core
    private Shop() { }

    public static Shop Create(string name, string hashedApiKey, string defaultLanguageCode)
    {
        // Add validation/guard clauses here
        return new Shop
        {
            Name = name,
            HashedApiKey = hashedApiKey,
            DefaultLanguageCode = defaultLanguageCode,
            CreatedAt = DateTime.UtcNow
        };
    }
}