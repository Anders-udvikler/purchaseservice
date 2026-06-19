
using service.interfaces;
using Purchase.Models;
using service;
using service.Grapql;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("YourDatabaseName");
});


// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<EventEnvelopeService<Order>>();
builder.Services.AddScoped<StripeService>();
builder.Services.AddScoped<ProcessedEventService>();
builder.Services.AddHttpClient();

// RabbitMQ
builder.Services.AddSingleton<IRabbitPublisher, RabbitPublisher>();

// Background Worker
builder.Services.AddHostedService<PurchaseConsumerWorker>();

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
