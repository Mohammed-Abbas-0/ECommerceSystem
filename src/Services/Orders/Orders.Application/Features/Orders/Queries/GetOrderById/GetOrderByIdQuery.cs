using MediatR;
using Orders.Application.DTOs;

namespace Orders.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;