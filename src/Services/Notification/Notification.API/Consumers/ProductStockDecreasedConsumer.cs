using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs;
using Shared.Events.Events;

namespace Notification.API.Consumers;

public class ProductStockDecreasedConsumer : IConsumer<ProductStockDecreasedEvent>
{
    private readonly ILogger<ProductStockDecreasedConsumer> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProductStockDecreasedConsumer(
        ILogger<ProductStockDecreasedConsumer> logger,
        IHubContext<NotificationHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
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
        
        await _hubContext.Clients
           .Group("admins")
           .SendAsync("LowStockAlert", new
           {
               product.ProductId,
               product.ProductName,
               product.NewStock,
               Message = $"⚠️ Low Stock Alert: {product.ProductName} has only {product.NewStock} items left!"
           });
        //await Task.CompletedTask;
    }
}