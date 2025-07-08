// Path: SaloonOS.Api/Controllers/CustomersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Features.Booking.Commands;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("get-or-create")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> GetOrCreate([FromBody] GetOrCreateCustomerByTelegramIdCommand command)
    {
        var customerDto = await _mediator.Send(command);
        // We can't easily tell if it was GET or CREATE, so returning 200 OK is a safe, idempotent response.
        return Ok(customerDto);
    }
}