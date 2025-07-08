// Path: SaloonOS.Application/DTOs/AdminAppointmentDto.cs
namespace SaloonOS.Application.DTOs;

/// <summary>
/// A detailed DTO for displaying appointment information to a shop owner (admin).
/// It includes details about the customer, service, and staff that are not needed
/// in the customer-facing AppointmentDto.
/// </summary>
public class AdminAppointmentDto
{
    public Guid Id { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string Status { get; init; } = string.Empty;

    // Denormalized data for easy display
    public string CustomerName { get; init; } = "N/A"; // In real system, this comes from the Customer entity
    public string ServiceName { get; init; } = "N/A";
    public string StaffName { get; init; } = "N/A";
    public decimal Price { get; init; }
}