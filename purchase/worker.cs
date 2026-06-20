using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Purchase.Models;
using Purchase.Enums;
using service;
using models;
using service.Grapql;

public class PurchaseConsumerWorker : BackgroundService
{
    private readonly ILogger<PurchaseConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    private IConnection? _connection;
    private IModel? _channel;

    public PurchaseConsumerWorker(
        ILogger<PurchaseConsumerWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: "purchase_queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        _logger.LogInformation("PurchaseConsumerWorker started");

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var evt = JsonSerializer.Deserialize<EventEnvelope<Order>>(json);

                if (evt?.payload == null)
                {
                    _logger.LogWarning("Invalid message received");
                    return;
                }

                using var scope = _scopeFactory.CreateScope();

                var processedService = scope.ServiceProvider.GetRequiredService<IProcessedEventService>();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                if (await processedService.AlreadyProcessed(evt.eventId))
                {
                    _logger.LogInformation("Event already processed: {Id}", evt.eventId);
                    return;
                }

                // Example HTTP call to API Gateway
                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsync(
                    "http://api-gateway:8080/purchase",
                    new StringContent(
                        JsonSerializer.Serialize(evt.payload),
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed: {Status}", response.StatusCode);
                    return;
                }

                await processedService.MarkProcessed(evt.eventId);

                _logger.LogInformation("Processed purchase event {Id}", evt.eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        };

        _channel.BasicConsume(
            queue: "purchase_queue",
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}