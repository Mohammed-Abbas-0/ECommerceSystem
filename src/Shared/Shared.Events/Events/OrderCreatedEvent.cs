namespace Shared.Events.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    string CustomerId,
    decimal TotalAmount,
    DateTime CreatedAt
);