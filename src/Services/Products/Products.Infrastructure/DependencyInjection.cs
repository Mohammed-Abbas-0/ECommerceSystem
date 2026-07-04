using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Domain.Interfaces;
using Products.Infrastructure.Data;
using Products.Infrastructure.Jobs;
using Products.Infrastructure.Messaging;
using Products.Infrastructure.Repositories;
using Products.Infrastructure.Services;

namespace Products.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        services.AddHangfireServer();

        // MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]);
                    h.Password(configuration["RabbitMQ:Password"]);
                });
            });
        });

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Services
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<EventPublisher>();

        // Jobs
        services.AddScoped<StockCheckJob>();
        services.AddScoped<DailyReportJob>();

        return services;
    }
}