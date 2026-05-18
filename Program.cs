using Furnitureservice;
using HotChocolate.Execution;
using MongoDB.Driver;
using UserService;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<FurnitureService>();
builder.Services.AddSingleton<ReportService>();

builder.Services.AddGraphQLServer()
    .AddQueryType<query.Query>()
    .AddMutationType<mutation.Mutation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.Run();

