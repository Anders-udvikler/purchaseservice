using System.Text;
using System.Text.Json;
using models;
using Purchase.Enums;
using Purchase.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using service;
using service.Grapql;

public class PurchaseConsumerWorker : BackgroundService
{
    private readonly ILogger<PurchaseConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HttpClient _httpClient;

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public PurchaseConsumerWorker(
        ILogger<PurchaseConsumerWorker> logger,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClient = httpClientFactory.CreateClient();

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

        _channel.BasicQos(0, 10, false);
    }

protected override Task ExecuteAsync(CancellationToken stoppingToken)
{
    var consumer = new AsyncEventingBasicConsumer(_channel);

    consumer.Received += async (sender, ea) =>
    {
        using var scope = _scopeFactory.CreateScope();

        var processed = scope.ServiceProvider.GetRequiredService<ProcessedEventService>();
        var envelope = scope.ServiceProvider.GetRequiredService<EventEnvelopeService<Order>>();

        var json = Encoding.UTF8.GetString(ea.Body.ToArray());

        EventEnvelope<Order>? evt;

        try
        {
            evt = JsonSerializer.Deserialize<EventEnvelope<Order>>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize message");
            _channel.BasicAck(ea.DeliveryTag, false);
            return;
        }

        if (evt == null)
        {
            _channel.BasicAck(ea.DeliveryTag, false);
            return;
        }

        try
        {
            // =========================
            // IDEMPOTENCY CHECK
            // =========================
            if (await processed.AlreadyProcessed(evt.eventId))
            {
                _logger.LogInformation("Skipping duplicate event {EventId}", evt.eventId);
                _channel.BasicAck(ea.DeliveryTag, false);
                return;
            }

            // =========================
            // BUSINESS LOGIC
            // =========================
            if (evt.eventType == "PurchaseCompleted")
            {
                _logger.LogInformation("Processing PurchaseCompleted {EventId}", evt.eventId);

                evt.payload.OrderStatus = OrderStatus.Completed;

                await envelope.UpdateFurniture(evt.eventId, evt.payload);

                var jsonPayload = JsonSerializer.Serialize(evt.payload);

                var response = await _httpClient.PostAsync(
                    "http://api:8080/createpurchase",
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("HTTP call failed: {Status}", response.StatusCode);
                }
            }
            else if (evt.eventType == "PurchaseFailed")
            {
                _logger.LogInformation("Processing PurchaseFailed {EventId}", evt.eventId);

                evt.payload.OrderStatus = OrderStatus.Cancelled;

                await envelope.UpdateFurniture(evt.eventId, evt.payload);
            }
            else
            {
                _logger.LogWarning("Unknown event type {Type}", evt.eventType);
            }

            // =========================
            // MARK AS PROCESSED
            // =========================
            await processed.MarkProcessed(evt.eventId);

            _channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {EventId}", evt.eventId);

            _channel.BasicNack(ea.DeliveryTag, false, true);
        }
    };

    _channel.BasicConsume(
        queue: "purchase_queue",
        autoAck: false,
        consumer: consumer);

    return Task.CompletedTask;
}

    public override void Dispose()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();

            _channel?.Dispose();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }

        base.Dispose();
    }
}