// Path: SaloonOS.Api/Controllers/ConfigurationController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Features.TenantManagement.Queries;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConfigurationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// The primary bootstrap endpoint for a bot client.
    /// It returns all necessary static data for the bot to initialize its UI and state.
    /// This endpoint is protected by API Key authentication.
    /// </summary>
    [HttpGet("bot")]
    [ProducesResponseType(typeof(BotConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBotConfiguration()
    {
        var config = await _mediator.Send(new GetBotConfigurationQuery());
        return Ok(config);
    }
}