using Orders.Domain.Common;
using Orders.Domain.Enums;

namespace Orders.Domain.Entities;

public class Order : BaseEntity
{
    public List<OrderItem> Items { get; private set; } = new();

    public string CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);

    private Order() { }

    public static Order Create(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("CustomerId is required");

        return new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending
        };
    }

    public void AddItem(Guid productId, string productName, decimal price, int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
            throw new InvalidOperationException("Product already exists in order");

        var item = OrderItem.Create(productId, productName, price, quantity);
        Items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus status)
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot update a cancelled order");

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered order");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}