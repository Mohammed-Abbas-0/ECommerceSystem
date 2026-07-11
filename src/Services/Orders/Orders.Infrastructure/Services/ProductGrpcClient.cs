using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orders.Domain.Interfaces;
using Orders.Infrastructure.Protos;
using Orders.Infrastructure.Resilience;
using Polly.CircuitBreaker;

namespace Orders.Infrastructure.Services;

public class ProductGrpcClient : IProductGrpcClient
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _client;
    private readonly ILogger<ProductGrpcClient> _logger;
    private readonly Polly.IAsyncPolicy _policy;

    public ProductGrpcClient(
        IConfiguration configuration,
        ILogger<ProductGrpcClient> logger)
    {
        _logger = logger;
        _policy = ResiliencePolicies.GetCombinedPolicy(logger);

        var channel = GrpcChannel.ForAddress(
            configuration["GrpcSettings:ProductServiceUrl"]
                ?? "http://localhost:5293",
            new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            });

        _client = new ProductGrpcService.ProductGrpcServiceClient(channel);
    }

    public async Task<bool> CheckStockAsync(Guid productId, int quantity)
    {
        try
        {
            return await _policy.ExecuteAsync(async () =>
            {
                var response = await _client.CheckStockAsync(new CheckStockRequest
                {
                    ProductId = productId.ToString(),
                    Quantity = quantity
                });

                _logger.LogInformation(
                    "✅ Stock Check: ProductId={ProductId}, Available={Available}",
                    productId,
                    response.IsAvailable);

                return response.IsAvailable;
            });
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError(
                "🔴 Circuit Breaker Open - Products Service unavailable");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Error checking stock for ProductId={ProductId}", productId);
            return false;
        }
    }
}