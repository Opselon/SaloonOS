using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SaloonOS.Api.Resources;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Features.TenantManagement.Commands;
using SaloonOS.Application.Features.TenantManagement.Queries;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ShopsController(IMediator mediator, IStringLocalizer<SharedResources> localizer)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
    {
        var shopId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetShopById), new { id = shopId }, shopId);
    }

    /// <summary>
    /// Gets the details for a specific shop.
    /// This endpoint is protected by the API Key Authentication middleware.
    /// </summary>
    /// <param name="id">The ID of the shop to retrieve.</param>
    /// <returns>The details of the shop.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShopDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShopById(Guid id)
    {
        try
        {
            var query = new GetShopByIdQuery(id);
            var shop = await _mediator.Send(query);

            return shop is not null ? Ok(shop) : NotFound(_localizer["ShopNotFound"].Value);
        }
        catch (InvalidOperationException ex) when (ex.Message == "ForbiddenAccess")
        {
            return Forbid(_localizer["ForbiddenAccess"].Value);
        }
        catch (UnauthorizedAccessException)
        {
            // This is a fallback. The middleware should handle this.
            return Unauthorized();
        }
    }
}