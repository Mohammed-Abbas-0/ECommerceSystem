using MediatR;

namespace Orders.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid Id) : IRequest<bool>;