using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Enums;

namespace Orders.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    Guid Id,
    OrderStatus Status
) : IRequest<OrderDto>;