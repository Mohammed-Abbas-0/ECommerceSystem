using MassTransit;
using Shared.Events.Events;

namespace Products.Infrastructure.Messaging;

public class EventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishStockDecreasedAsync(Guid productId, string productName, int newStock)
    {
        await _publishEndpoint.Publish(new ProductStockDecreasedEvent(
            productId,
            productName,
            newStock
        ));
    }
}