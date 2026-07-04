using MediatR;
using Orders.Domain.Interfaces;

namespace Orders.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _repository;

    public CancelOrderCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.Id);

        if (order is null)
            throw new KeyNotFoundException($"Order with Id {request.Id} not found");

        order.Cancel();
        await _repository.UpdateAsync(order);

        return true;
    }
}