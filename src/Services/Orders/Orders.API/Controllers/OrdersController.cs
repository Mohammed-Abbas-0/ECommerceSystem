using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.DTOs;
using Orders.Application.Features.Orders.Commands.CancelOrder;
using Orders.Application.Features.Orders.Commands.CreateOrder;
using Orders.Application.Features.Orders.Commands.UpdateOrderStatus;
using Orders.Application.Features.Orders.Queries.GetOrderById;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, UpdateOrderStatusCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        await _mediator.Send(new CancelOrderCommand(id));
        return NoContent();
    }
}