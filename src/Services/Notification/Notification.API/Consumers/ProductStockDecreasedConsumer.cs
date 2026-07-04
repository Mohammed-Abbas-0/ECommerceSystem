using MassTransit;
using Shared.Events.Events;

namespace Notification.API.Consumers;

public class ProductStockDecreasedConsumer : IConsumer<ProductStockDecreasedEvent>
{
    private readonly ILogger<ProductStockDecreasedConsumer> _logger;

    public ProductStockDecreasedConsumer(ILogger<ProductStockDecreasedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductStockDecreasedEvent> context)
    {
        var product = context.Message;

        _logger.LogWarning(
            "⚠️ Stock Decreased: ProductId={ProductId}, Name={Name}, NewStock={Stock}",
            product.ProductId,
            product.ProductName,
            product.NewStock
        );

        await Task.CompletedTask;
    }
}