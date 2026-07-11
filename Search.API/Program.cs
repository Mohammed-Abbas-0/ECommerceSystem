using MassTransit;
using Nest;
using Search.API.Consumers;
using Search.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Elasticsearch
var elasticUrl = builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
var settings = new ConnectionSettings(new Uri(elasticUrl))
    .DefaultIndex("products")
    .DefaultFieldNameInferrer(p => p.ToLower()); 

builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));
builder.Services.AddSingleton<ElasticsearchService>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var username = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();