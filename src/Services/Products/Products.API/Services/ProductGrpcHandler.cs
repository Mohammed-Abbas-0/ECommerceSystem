using Grpc.Core;
using Products.API.Protos;
using Products.Domain.Interfaces;

namespace Products.API.Services;

public class ProductGrpcHandler : ProductGrpcService.ProductGrpcServiceBase
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductGrpcHandler> _logger;

    public ProductGrpcHandler(
        IProductRepository repository,
        ILogger<ProductGrpcHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<GetProductResponse> GetProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        var product = await _repository.GetByIdAsync(Guid.Parse(request.Id));

        if (product is null)
            return new GetProductResponse { Found = false };

        return new GetProductResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = (double)product.Price,
            Stock = product.Stock,
            Found = true
        };
    }

    public override async Task<CheckStockResponse> CheckStock(
        CheckStockRequest request,
        ServerCallContext context)
    {
        var product = await _repository.GetByIdAsync(Guid.Parse(request.ProductId));

        if (product is null)
            return new CheckStockResponse
            {
                IsAvailable = false,
                CurrentStock = 0,
                Message = "Product not found"
            };

        var isAvailable = product.Stock >= request.Quantity;

        return new CheckStockResponse
        {
            IsAvailable = isAvailable,
            CurrentStock = product.Stock,
            Message = isAvailable
                ? "Stock is available"
                : $"Not enough stock. Available: {product.Stock}"
        };
    }
}