using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orders.Domain.Interfaces;
using Orders.Infrastructure.Protos; 

namespace Orders.Infrastructure.Services;

public class ProductGrpcClient : IProductGrpcClient
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _client;
    private readonly ILogger<ProductGrpcClient> _logger;

    public ProductGrpcClient(
        IConfiguration configuration,
        ILogger<ProductGrpcClient> logger)
    {
        _logger = logger;

        var channel = GrpcChannel.ForAddress(
                    configuration["GrpcSettings:ProductServiceUrl"]
                        ?? "http://localhost:5292",
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
            var response = await _client.CheckStockAsync(new CheckStockRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            });

            _logger.LogInformation(
                "Stock Check: ProductId={ProductId}, Available={Available}",
                productId,
                response.IsAvailable);

            return response.IsAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error checking stock for ProductId={ProductId}", productId);
            return false;
        }
    }
}