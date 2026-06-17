using Furnitureservice;
using service.interfaces;
using Workers;
using Purchase.Models;
using service;

var builder = WebApplication.CreateBuilder(args);

// Controllers (REST API)
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Services
builder.Services.AddSingleton<FurnitureService>();
builder.Services.AddSingleton<OrderService>();

// RabbitMQ
builder.Services.AddSingleton<IRabbitPublisher, RabbitPublisher>();

// Background Worker
builder.Services.AddHostedService<Worker>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<query.Query>()
    .AddMutationType<mutation.Mutation>();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapOpenApi();
}

// HTTPS (optional)
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// REST Controllers
app.MapControllers();

// GraphQL endpoint
app.MapGraphQL();

app.Run();
