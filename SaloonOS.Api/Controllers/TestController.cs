using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SaloonOS.Api.Resources;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public TestController(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }

    [HttpGet("welcome")]
    public IActionResult GetWelcomeMessage()
    {
        // This will automatically pick the string from the correct .resx file
        // based on the "Accept-Language" header sent by the client.
        var localizedMessage = _localizer["WelcomeMessage"];
        return Ok(new { Message = localizedMessage.Value });
    }
}