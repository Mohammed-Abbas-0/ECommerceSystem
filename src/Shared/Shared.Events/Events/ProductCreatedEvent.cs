namespace Shared.Events.Events;

public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category
);