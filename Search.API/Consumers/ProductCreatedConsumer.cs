using MassTransit;
using Search.API.Models;
using Search.API.Services;
using Shared.Events.Events;

namespace Search.API.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ElasticsearchService _elasticsearchService;
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(
        ElasticsearchService elasticsearchService,
        ILogger<ProductCreatedConsumer> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var product = context.Message;

        _logger.LogInformation(
            "🔍 Indexing Product: {Name}", product.Name);

        await _elasticsearchService.IndexProductAsync(new ProductDocument
        {
            Id = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category
        });

        _logger.LogInformation(
            "✅ Product Indexed: {Name}", product.Name);
    }
}