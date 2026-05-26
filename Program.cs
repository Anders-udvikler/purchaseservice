using Furnitureservice;
using HotChocolate.Execution;
using MongoDB.Driver;
using UserService;
using service.interfaces;
using Workers;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Services
builder.Services.AddSingleton<FurnitureService>();
builder.Services.AddSingleton<ReportService>();

// RabbitMQ
builder.Services.AddSingleton<IRabbitPublisher, RabbitPublisher>();

builder.Services.AddHostedService<Worker>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<query.Query>()
    .AddMutationType<mutation.Mutation>();

var app = builder.Build();

// OpenAPI UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

