using AutoMapper;
using Orders.Application.DTOs;
using Orders.Domain.Entities;

namespace Orders.Application.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}