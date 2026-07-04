using Orders.Domain.Common;

namespace Orders.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => Price * Quantity;

    private OrderItem() { }

    public static OrderItem Create(Guid productId, string productName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        };
    }
}