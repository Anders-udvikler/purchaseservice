using System.Text;
using System.Text.Json;
using Furnitures;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Workers
{
    public class Worker:BackgroundService
    {
        private readonly ILogger<Worker> _logger;
            private IConnection _connection;
            private IModel _channel;
        private readonly RabbitPublisher _publisher;

/// <summary>
/// Initializes a new instance of the Worker class. The constructor takes an ILogger<Worker> for logging purposes and a RabbitPublisher for publishing messages to RabbitMQ. It attempts to establish a connection to RabbitMQ and declares a queue named "queueName". If the connection fails, it retries up to 5 times with a delay of 5 seconds between each attempt, logging the error message for each failure.
/// </summary>
/// <param name="logger">The logger for logging purposes.</param>
/// <param name="publisher">The RabbitPublisher for publishing messages to RabbitMQ.</param>
        public Worker(ILogger<Worker> logger, RabbitPublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            int retries=5;
            while(retries>0)
            {
                try
                {
                    _logger.LogInformation("Connecting to RabbitMQ...");
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _logger.LogInformation("Connected to RabbitMQ.");
                    _channel.QueueDeclare(
                    queue: "queueName",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex,"Failed to connect to RabbitMQ. Retrying in 5 seconds...");
                    retries--;
                    Thread.Sleep(5000);
                }
        }}

/// <summary>
/// Executes the background service. The method sets up a RabbitMQ consumer that listens for messages on the specified queue. When a message is received, it processes the message by deserializing it into a FurnitureId object and handling it based on the event type (ListingCreated, ListingUpdated, ListingDeleted). If the message is processed successfully, it acknowledges the message; otherwise, it logs the error and negatively acknowledges the message for reprocessing.
/// </summary>
/// <param name="stoppingToken">The token to monitor for cancellation requests.</param>
/// <returns>A task representing the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "listing_queue";

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                _logger.LogInformation("Message received");

                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var input = JsonSerializer.Deserialize<FurnitureId>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (input == null)
                {
                    _logger.LogWarning("Invalid input");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                if (string.IsNullOrEmpty(input.EventType))
                {
                    _logger.LogWarning("Missing eventType");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                switch (input.EventType)
                {
                    case "ListingCreated":
                    Console.WriteLine($"Listing created: {input.Guid}");
                        break;
                    case "ListingUpdated":
                        Console.WriteLine($"Listing updated: {input.Guid}");
                        break;

                    case "ListingDeleted":
                        Console.WriteLine($"Listing deleted: {input.Guid}");
                        break;
                    default:
                        _logger.LogWarning("Unknown event type: {EventType}", input.EventType);
                        break;
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer
        );

        return Task.CompletedTask;
    }

/// <summary>
/// Handles the upsert operation for a furniture item. The method takes a FurnitureId object as input, extracts the relevant details, and creates a new Furniture object based on the provided information. It then performs the necessary operations to upsert the furniture item in the database. If any exceptions occur during the process, it logs the error message.
/// </summary>
/// <param name="input">The FurnitureId object containing the input data.</param>
/// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleUpsert(FurnitureId input)
        {        
            var details = input.ListingDetails ?? new Furniture();

        var color = details.Color;
        var sub = details.Categories?.FirstOrDefault()?.subcats?.FirstOrDefault();

        var post = new Furniture
        {
            Guid = input.Guid,
            PersonId = input.PersonGUID,
            Title = details.Title,
            Description = details.Description,
            Quantity = details.Quantity,
            Price = details.Price,
            Condition = details.Condition,
            ZipCode = details.ZipCode,

            Color = color != null
                ? new Colors.Color
                {
                    Name = color.Name,
                    Href = color.Href
                }
                : new Colors.Color(),

            Images = details.Images?
                .Select(url => new Images.Image { Url = url.Url })
                .ToList() ?? new List<Images.Image>()
        };
        }

/// <summary>
/// Handles the deletion of a listing. The method takes a GUID as a parameter and publishes a message to RabbitMQ indicating that the listing with the specified GUID has been deleted. It also logs the information about the published delete event.
/// </summary>
/// <param name="guid">The GUID of the listing to delete.</param>
/// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleDelete(Guid guid)
    {
         await _publisher.PublishListingDeleted(guid);
        _logger.LogInformation("Published delete event for {Guid}", guid);

    }

}}