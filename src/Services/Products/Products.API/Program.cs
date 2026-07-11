using Hangfire;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Products.API.Services;
using Products.Application;
using Products.Infrastructure;
using Products.Infrastructure.Jobs;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    // بنقوله اشتغل على Port 5292 بـ HTTP/1.1
    options.ListenLocalhost(5292, o =>
        o.Protocols = HttpProtocols.Http1);

    // واشتغل على Port 5293 بـ HTTP/2 للـ gRPC
    options.ListenLocalhost(5293, o =>
        o.Protocols = HttpProtocols.Http2);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// gRPC
builder.Services.AddGrpc();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<StockCheckJob>(
    "stock-check",
    job => job.CheckLowStockAsync(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<DailyReportJob>(
    "daily-report",
    job => job.GenerateReportAsync(),
    Cron.Daily);

//app.UseHttpsRedirection(); so Grpc
app.UseAuthorization();
app.MapControllers();

// gRPC Endpoint
app.MapGrpcService<ProductGrpcHandler>();

app.Run();