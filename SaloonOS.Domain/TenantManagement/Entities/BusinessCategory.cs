using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.TenantManagement.Entities;

/// <summary>
/// Represents the type of business a shop operates. This is crucial for future
/// filtering, feature-flagging, and providing a tailored experience.
/// For simplicity and performance, this could be an Enum, but an entity allows for
/// dynamic addition of categories without recompiling the application.
/// </summary>
public class BusinessCategory : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
}