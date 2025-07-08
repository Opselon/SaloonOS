// Path: SaloonOS.Api/Controllers/ScheduleController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.Features.Booking.Commands;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/schedule")] // Or /api/admin/schedule
public class ScheduleController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("work-schedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetWorkSchedule([FromBody] SetWorkScheduleCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // GET endpoint would go here...
}