using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Features.Booking.Commands;
using SaloonOS.Application.Features.Booking.Queries;
using System.Globalization;
// The folder SaloonOS.Application/Features/Booking/Queries was not created.
// Once it is, and ListServicesByShopQuery is created, this 'using' will be valid.
// For now, we comment it out to allow the project to build.
// using SaloonOS.Application.Features.Booking.Queries; 

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ... CreateService method is fine ...
    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceCommand command)
    {
        var serviceDto = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, serviceDto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListServices()
    {
        // Get the language from the request context, which was set by the localization middleware.
        var languageCode = CultureInfo.CurrentUICulture.Name;

        var query = new ListServicesByShopQuery(languageCode);
        var services = await _mediator.Send(query);
        return Ok(services);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceCommand request)
    {
        // We must create the command with the id from the route to be robust.
        var command = new UpdateServiceCommand(id, request.Name, request.Description, request.Price, request.DurationInMinutes);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        await _mediator.Send(new DeleteServiceCommand(id));
        return NoContent();
    }
}