// Path: SaloonOS.Api/Controllers/PaymentsController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaloonOS.Application.Exceptions;
using SaloonOS.Application.Features.Payments.Commands;

namespace SaloonOS.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("process")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        try
        {
            var transactionId = await _mediator.Send(command);
            return Ok(new { TransactionId = transactionId });
        }
        catch (PaymentException ex)
        {
            // A specific error from our payment service (e.g., Stripe card error).
            return BadRequest(new { Error = "PaymentFailed", Details = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}