namespace Orders.Domain.Interfaces;

public interface IProductGrpcClient
{
    Task<bool> CheckStockAsync(Guid productId, int quantity);
}