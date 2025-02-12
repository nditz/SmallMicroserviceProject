using Core.Catalog.Service.Entities;
using Core.Common.MassTransit;
using Core.Common.MongoDB;
using Core.Common.Settings;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuration
var configuration = builder.Configuration;
var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();


builder.Services.AddMongo()
                    .AddMongoRepository<Item>("items")
                    .AddMassTransitWithRabbitMq();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core.Catalog.Service", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core.Catalog.Service v1"));

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

app.UseRouting();

app.UseAuthorization();

app.Run();
