using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Purchase.Enums;
using Purchase.Models;
using service;
using models;
using service.Grapql;

public class PurchaseConsumerWorker : BackgroundService
{
    private readonly ILogger<PurchaseConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    private IConnection _connection;
    private IModel _channel;

    private const string MainQueue = "purchase_queue";
    private const string RetryQueue = "purchase_retry_queue";
    private const string DlqQueue = "purchase_dlq";

    private const int MaxRetries = 5;

    public PurchaseConsumerWorker(
        ILogger<PurchaseConsumerWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;

        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            TopologyRecoveryEnabled = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // MAIN QUEUE
        _channel.QueueDeclare(
            queue: MainQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = RetryQueue
            });

        // RETRY QUEUE
        _channel.QueueDeclare(
            queue: RetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-message-ttl"] = 5000, // base delay
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = MainQueue
            });

        // DLQ
        _channel.QueueDeclare(
            queue: DlqQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // BACKPRESSURE CONTROL
        _channel.BasicQos(0, 5, false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, ea) =>
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            int retryCount = GetRetryCount(ea);

            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<EventEnvelope<Order>>(json);

                if (evt == null)
                    throw new Exception("Invalid message");

                using var scope = _scopeFactory.CreateScope();

                var processed = scope.ServiceProvider.GetRequiredService<ProcessedEventService>();
                var envelope = scope.ServiceProvider.GetRequiredService<EventEnvelopeService<Order>>();
                var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();

                // IDEMPOTENCY (must be atomic in DB ideally)
                if (!await processed.AlreadyProcessed(evt.eventId))
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                var http = _httpClientFactory.CreateClient();

                if (evt.eventType == "ItemReserved")
                {
                    evt.payload.OrderStatus = OrderStatus.Completed;

                    await envelope.UpdateFurniture(evt.eventId, evt);

                    var response = await http.PostAsync(
                        "http://api:8080/createpurchase",
                        new StringContent(JsonSerializer.Serialize(evt.payload),
                        Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"API failed: {response.StatusCode}");
                }
                else if (evt.eventType == "ItemReservedFailed")
                {
                    evt.payload.OrderStatus = OrderStatus.Cancelled;

                    await envelope.UpdateFurniture(evt.eventId, evt);
                    await orderService.UpdateOrder(evt.payload);
                }
                else
                {
                    _logger.LogWarning("Unknown event type {Type}", evt.eventType);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed");

                retryCount++;

                if (retryCount > MaxRetries)
                {
                    MoveToDlq(ea.Body);

                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                PublishToRetry(ea.Body, retryCount);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
        };

        _channel.BasicConsume(
            queue: MainQueue,
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private int GetRetryCount(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties?.Headers == null)
            return 0;

        if (ea.BasicProperties.Headers.TryGetValue("retry-count", out var value))
            return Convert.ToInt32(value);

        return 0;
    }

    private void PublishToRetry(ReadOnlyMemory<byte> body, int retryCount)
    {
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        props.Headers = new Dictionary<string, object>
        {
            ["retry-count"] = retryCount
        };

        var delay = (int)Math.Pow(2, retryCount) * 1000;

        props.Headers["x-delay"] = delay;

        _channel.BasicPublish(
            exchange: "",
            routingKey: RetryQueue,
            basicProperties: props,
            body: body);
    }

    private void MoveToDlq(ReadOnlyMemory<byte> body)
    {
        _channel.BasicPublish(
            exchange: "",
            routingKey: DlqQueue,
            basicProperties: null,
            body: body);

        _logger.LogWarning("Message moved to DLQ");
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
            _logger.LogError(ex, "Dispose error");
        }

        base.Dispose();
    }
}