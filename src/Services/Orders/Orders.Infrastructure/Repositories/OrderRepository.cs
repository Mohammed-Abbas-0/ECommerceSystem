using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    static OrderRepository()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }

    public OrderRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _orders = database.GetCollection<Order>(settings.Value.CollectionName);
    }
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orders.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId)
    {
        return await _orders.Find(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task<Order> AddAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
        return order;
    }

    public async Task UpdateAsync(Order order)
    {
        await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _orders.DeleteOneAsync(o => o.Id == id);
    }
}