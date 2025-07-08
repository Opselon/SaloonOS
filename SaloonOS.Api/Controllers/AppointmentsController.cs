using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Features.Booking.Commands;
using SaloonOS.Application.Features.Booking.Queries;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("slots")]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] GetAvailableTimeSlotsQuery query)
    {
        var slots = await _mediator.Send(query);
        return Ok(slots);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentCommand command)
    {
        try
        {
            var appointmentDto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentDto.Id }, appointmentDto);
        }
        catch (InvalidOperationException ex) when (ex.Message == "TimeSlotNotAvailable")
        {
            // Using a ProblemDetails response is more modern and informative.
            return new ConflictObjectResult(new ProblemDetails
            {
                Title = "Time Slot Conflict",
                Detail = "The requested time slot is no longer available. Please select another time."
            });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        // This would be a new Query: GetAppointmentByIdQuery
        await Task.CompletedTask;
        return Ok($"Endpoint to get appointment {id} is not yet implemented.");
    }
}