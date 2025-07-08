// Path: SaloonOS.Application/DTOs/StaffMemberDto.cs
namespace SaloonOS.Application.DTOs;

/// <summary>
/// A Data Transfer Object representing the public information for a Staff Member.
/// </summary>
public class StaffMemberDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}