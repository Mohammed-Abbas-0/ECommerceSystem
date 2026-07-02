using Products.Domain.Common;

namespace Products.Domain.Entities;

// Rich domain model for Product entity with encapsulation and business logic
public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string Category { get; private set; }

    private Product() { }

    // Factory Method
    public static Product Create(string name, string description, decimal price, int stock, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative");

        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            Category = category
        };
    }

    public void Update(string name, string description, decimal price, string category)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity > Stock)
            throw new ArgumentException("Not enough stock");

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncreaseStock(int quantity)
    {
        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}