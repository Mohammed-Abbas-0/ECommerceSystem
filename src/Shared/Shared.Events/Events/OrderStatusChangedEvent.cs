namespace Shared.Events.Events;

public record OrderStatusChangedEvent(
    Guid OrderId,
    string CustomerId,
    string OldStatus,
    string NewStatus
);