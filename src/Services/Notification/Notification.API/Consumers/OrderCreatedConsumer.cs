using MassTransit;
using Shared.Events.Events;

namespace Notification.API.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation(
            "📦 Order Created: OrderId={OrderId}, CustomerId={CustomerId}, Total={Total}",
            order.OrderId,
            order.CustomerId,
            order.TotalAmount
        );

        await Task.CompletedTask;
    }
}