using System.Collections;
using System.Text;
using System.Text.Json;
using Furnitures;
using models;
using mutation;
using Purchase.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using service.Grapql;
using service.interfaces;


namespace Workers
{
    public class Worker:BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly IRabbitPublisher _publisher;

        private readonly EventEnvelopeService _repo;

/// <summary>
/// Initializes a new instance of the Worker class. The constructor takes an ILogger<Worker> for logging purposes and a RabbitPublisher for publishing messages to RabbitMQ. It attempts to establish a connection to RabbitMQ and declares a queue named "queueName". If the connection fails, it retries up to 5 times with a delay of 5 seconds between each attempt, logging the error message for each failure.
/// </summary>
/// <param name="logger">The logger for logging purposes.</param>
/// <param name="publisher">The RabbitPublisher for publishing messages to RabbitMQ.</param>
/// 
public Worker(ILogger<Worker> logger, IRabbitPublisher publisher,EventEnvelopeService repo)
{
    _logger = logger;
    _publisher = publisher;
    _repo = repo;
    Console.WriteLine("Worker constructor called");
}



/// <summary>
/// Executes the background service. The method sets up a RabbitMQ consumer that listens for messages on the specified queue. When a message is received, it processes the message by deserializing it into a FurnitureId object and handling it based on the event type (ListingCreated, ListingUpdated, ListingDeleted). If the message is processed successfully, it acknowledges the message; otherwise, it logs the error and negatively acknowledges the message for reprocessing.
/// </summary>
/// <param name="stoppingToken">The token to monitor for cancellation requests.</param>
/// <returns>A task representing the asynchronous operation.</returns>
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var factory = new ConnectionFactory
    {
        HostName = "rabbitmq",
        DispatchConsumersAsync = true
    };

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("listing_queue", true, false, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var input = JsonSerializer.Deserialize<EventEnvelope<Order>>(json);
                await HandleEvent(input);
            };

            channel.BasicConsume("listing_queue", false, consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker crashed, retrying...");
            await Task.Delay(3000, stoppingToken);
        }
    }
}

private async Task HandleEvent(EventEnvelope<Order> input)
{

    EventEnvelope<Order> order =await _repo.GetEventById(input.eventId);
    if(order!=null)
    {
           _logger.LogInformation("" + input.eventId);     
    }
    switch (input.eventType)
    {

        case "ItemReserved":
            await HandleUpdated(input);
            break;
        case "ItemReservedFailed":
            await HandleDeleted(input);
            break;
        default:
            _logger.LogWarning("Unknown event type: {EventType}", input.eventType);
            break;
    }
}
private async Task HandleCreated(EventEnvelope<Order> input)
{
    _logger.LogInformation("Created: {Guid}", input.causationId);
    switch(input.eventVersion)
            {
                case 1:
                await _repo.Addevent(input);
                   break;
                case 2:
                await _repo.Addevent(input);
                   break;
                default:
                  _logger.LogWarning( "" );
                  break;
            }
}

private async Task HandleUpdated(EventEnvelope<Order> input)
{
    await _repo.Addevent(input);
    switch (input.eventVersion)
    {
        case 1:
            break;
        case 2:
        break;
        default:
            _logger.LogWarning("Unknown event type: {EventType}", input.eventType);
            break;
    }
}

private async Task HandleDeleted(EventEnvelope<Order> input)
{
    await _repo.Addevent(input);
    switch (input.eventVersion)
    {
        case 1:
            break;
        case 2:
            break;
        default:
            _logger.LogWarning("Unknown event type: {EventType}", input.eventType);
            break;
    }
}
/// <summary>
/// Handles the deletion of a listing. The method takes a GUID as a parameter and publishes a message to RabbitMQ indicating that the listing with the specified GUID has been deleted. It also logs the information about the published delete event.
/// </summary>
/// <param name="guid">The GUID of the listing to delete.</param>
/// <returns>A task representing the asynchronous operation.</returns>

}}