using Microsoft.Extensions.Logging;
using Products.Domain.Interfaces;

namespace Products.Infrastructure.Jobs;

public class StockCheckJob
{
    private readonly IProductRepository _repository;
    private readonly ILogger<StockCheckJob> _logger;

    public StockCheckJob(IProductRepository repository, ILogger<StockCheckJob> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task CheckLowStockAsync()
    {
        var products = await _repository.GetAllAsync();

        var lowStockProducts = products.Where(p => p.Stock < 10).ToList();

        if (!lowStockProducts.Any())
        {
            _logger.LogInformation("✅ No low stock products found");
            return;
        }

        foreach (var product in lowStockProducts)
        {
            _logger.LogWarning(
                "⚠️ Low Stock Alert: Product {Name} has only {Stock} items left",
                product.Name,
                product.Stock);
        }
    }
}