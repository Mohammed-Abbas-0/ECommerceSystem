namespace Shared.Events.Events;

public record ProductStockDecreasedEvent(
    Guid ProductId,
    string ProductName,
    int NewStock
);