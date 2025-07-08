using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.Features.TenantManagement.Commands;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShopsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Creates a new shop tenant.
    /// </summary>
    /// <param name="command">The command containing the shop creation data.</param>
    /// <returns>The ID of the newly created shop.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
    {
        // The controller's job is minimal: delegate the work to MediatR.
        // The ValidationBehavior will automatically validate the command.
        // The CreateShopCommandHandler will execute the business logic.
        var shopId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetShopById), new { id = shopId }, shopId);
    }

    // Placeholder for a future GET endpoint
    [HttpGet("{id:guid}")]
    public IActionResult GetShopById(Guid id)
    {
        // This would be implemented with a MediatR Query.
        return Ok($"Endpoint to get shop with ID {id} is not yet implemented.");
    }
}