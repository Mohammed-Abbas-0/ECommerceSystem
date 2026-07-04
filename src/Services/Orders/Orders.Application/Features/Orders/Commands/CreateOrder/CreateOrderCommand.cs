using MediatR;
using Orders.Application.DTOs;

namespace Orders.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity
);

public record CreateOrderCommand(
    string CustomerId,
    List<CreateOrderItemRequest> Items
) : IRequest<OrderDto>;