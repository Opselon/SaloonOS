namespace SaloonOS.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; init; }
    public DateTime StartTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid ServiceId { get; init; }
    public Guid StaffId { get; init; }
}