using Microsoft.Extensions.Logging;
using Products.Domain.Interfaces;

namespace Products.Infrastructure.Jobs;

public class DailyReportJob
{
    private readonly IProductRepository _repository;
    private readonly ILogger<DailyReportJob> _logger;

    public DailyReportJob(IProductRepository repository, ILogger<DailyReportJob> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task GenerateReportAsync()
    {
        var products = await _repository.GetAllAsync();

        var report = products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                TotalStock = g.Sum(p => p.Stock)
            });

        _logger.LogInformation("📊 Daily Report - {Date}", DateTime.UtcNow.ToString("yyyy-MM-dd"));

        foreach (var item in report)
        {
            _logger.LogInformation(
                "Category: {Category} | Products: {Count} | Total Stock: {Stock}",
                item.Category,
                item.Count,
                item.TotalStock);
        }
    }
}