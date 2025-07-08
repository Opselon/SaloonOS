using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Exceptions;
using SaloonOS.Application.Features.Booking.Commands;
using SaloonOS.Application.Features.Booking.Queries;
using System.Data;

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


    [HttpGet("my-appointments/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerAppointments(Guid customerId)
    {
        var query = new GetCustomerAppointmentsQuery(customerId);
        var appointments = await _mediator.Send(query);
        return Ok(appointments);
    }

    [HttpDelete("{id:guid}/cancel/{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelAppointment(Guid id, Guid customerId)
    {
        try
        {
            var command = new CancelAppointmentCommand(id, customerId);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message == "CannotCancelAppointment")
        {
            return BadRequest(new { Message = "This appointment cannot be cancelled." });
        }
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

    [HttpPost("{id:guid}/admin/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] // For concurrency issues
    public async Task<IActionResult> CompleteAppointment(Guid id, [FromBody] long adminTelegramUserId)
    {
        try
        {
            await _mediator.Send(new CompleteAppointmentCommand(id, adminTelegramUserId));
            return NoContent();
        }
        catch (ConcurrencyException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/admin/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelAppointmentByAdmin(Guid id, [FromBody] long adminTelegramUserId)
    {
        try
        {
            await _mediator.Send(new CancelAppointmentByAdminCommand(id, adminTelegramUserId));
            return NoContent();
        }
        catch (ConcurrencyException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }


    [HttpGet("admin/schedule")]
    [ProducesResponseType(typeof(IEnumerable<AdminAppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDailySchedule([FromQuery] DateTime date, [FromQuery] long adminTelegramUserId)
    {
        try
        {
            var query = new GetAppointmentsForDayQuery(date, adminTelegramUserId);
            var schedule = await _mediator.Send(query);
            return Ok(schedule);
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "AdminNotAuthorized")
        {
            return Forbid("You are not authorized to view this schedule.");
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