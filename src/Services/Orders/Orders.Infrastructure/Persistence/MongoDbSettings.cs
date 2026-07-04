namespace Orders.Infrastructure.Persistence;

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public required string CollectionName { get; set; }
}