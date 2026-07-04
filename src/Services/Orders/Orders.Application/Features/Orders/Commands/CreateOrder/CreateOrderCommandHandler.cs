using AutoMapper;
using MassTransit;
using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Shared.Events.Events;

namespace Orders.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.CustomerId);

        foreach (var item in request.Items)
            order.AddItem(item.ProductId, item.ProductName, item.Price, item.Quantity);

        await _repository.AddAsync(order);

        await _publishEndpoint.Publish(new OrderCreatedEvent(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.CreatedAt
        ));

        return _mapper.Map<OrderDto>(order);
    }
}