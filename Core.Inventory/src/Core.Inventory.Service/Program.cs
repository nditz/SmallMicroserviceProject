using Core.Common.MongoDB;
using Core.Common.MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Core.Inventory.Service.Entities;
using Microsoft.OpenApi.Models;
using Core.Inventory.Service.Clients;
using Polly.Timeout;
using Polly;
using MongoDB.Driver;
using Core.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;
var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
Random jitterer = new Random();
var services = builder.Services;

services.AddMongo()
                    .AddMongoRepository<InventoryItem>("inventoryitems")
                    .AddMongoRepository<CatalogItem>("catalogitems")
                    .AddMassTransitWithRabbitMq();


services.AddHttpClient<CatalogClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
                }
            ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timespan) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
                },
                onReset: () =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Closing the circuit...");
                }
            ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

services.AddControllers();
services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core.Inventory.Service", Version = "v1" });
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core.Inventory.Service v1"));

    app.UseCors(corsBuilder =>
    {
        var allowedOrigin = builder.Configuration["AllowedOriginSetting"];
        if (!string.IsNullOrEmpty(allowedOrigin))
        {
            corsBuilder.WithOrigins(allowedOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();


