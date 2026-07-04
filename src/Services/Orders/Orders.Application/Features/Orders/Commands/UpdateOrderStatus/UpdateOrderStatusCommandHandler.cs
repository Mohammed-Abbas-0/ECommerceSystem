using AutoMapper;
using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Interfaces;

namespace Orders.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public UpdateOrderStatusCommandHandler(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.Id);

        if (order is null)
            throw new KeyNotFoundException($"Order with Id {request.Id} not found");

        order.UpdateStatus(request.Status);
        await _repository.UpdateAsync(order);

        return _mapper.Map<OrderDto>(order);
    }
}