using Microsoft.AspNetCore.Mvc;
using ShipIt.Api.Extensions;
using ShipIt.Application.Features;

namespace ShipIt.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderCommand command,
        [FromServices] CreateOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        var response = result.Value;
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetOrdersRequest request,
        [FromServices] GetOrdersHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] GetOrderByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new GetOrderByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }
}
