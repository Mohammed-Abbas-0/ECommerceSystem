using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs;
using Shared.Events.Events;

namespace Notification.API.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;


    public OrderCreatedConsumer(
       ILogger<OrderCreatedConsumer> logger,
       IHubContext<NotificationHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
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

        // Send Notification With SignalR
        await _hubContext.Clients
           .Group(order.CustomerId)
           .SendAsync("OrderCreated", new
           {
               order.OrderId,
               order.CustomerId,
               order.TotalAmount,
               Message = $"✅ Order {order.OrderId} has been created successfully!"
           });
        //await Task.CompletedTask;
    }
}