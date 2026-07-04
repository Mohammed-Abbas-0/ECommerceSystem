using Orders.Domain.Enums;

namespace Orders.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public required string CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}