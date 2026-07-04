using AutoMapper;
using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Interfaces;

namespace Orders.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.Id);

        if (order is null)
            throw new KeyNotFoundException($"Order with Id {request.Id} not found");

        return _mapper.Map<OrderDto>(order);
    }
}