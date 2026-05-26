using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using service.interfaces;

public class RabbitPublisher: IRabbitPublisher
{
    private readonly IModel _channel;
    private readonly string _queueName = "listing_queue";

    public RabbitPublisher()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.QueueDeclare(_queueName, false, false, false);
    }

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="message"></param>
/// <param name="routingKey"></param>
/// <returns></returns>
/// <exception cref="NotImplementedException"></exception>
    public Task PublishAsync<T>(T message, string routingKey)
    {
        throw new NotImplementedException();
    }

/// <summary>
/// Publishes a message to the RabbitMQ queue indicating that a listing has been deleted. The message contains the GUID of the deleted listing and an event type of "ListingDeleted".
/// </summary>
/// <param name="guid">The GUID of the deleted listing.</param>
/// <returns></returns>
    public async Task PublishListingDeleted(Guid guid)
    {
        var message = new
        {
            Guid = guid,
            EventType = "ListingDeleted"
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            "",
            _queueName,
            null,
            body
        );
    }

/// <summary>
/// Publishes a message to the RabbitMQ queue indicating that a listing has been updated. The message contains the GUID of the updated listing and an event type of "ListingUpdated".
/// </summary>
/// <param name="guid">The GUID of the updated listing.</param>
/// <returns></returns>
        public async Task PublishListingUpdated(Guid guid)
    {
        var message = new
        {
            Guid = guid,
            EventType = "ListingUpdated"
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            "",
            _queueName,
            null,
            body
        );
    }


/// <summary>
/// Publishes a message to the RabbitMQ queue indicating that a listing has been created. The message contains the GUID of the created listing and an event type of "ListingCreated".
/// </summary>
/// <param name="guid">The GUID of the created listing.</param>
/// <returns></returns>
    public async Task PublishListingCreated(Guid guid)
    {
        var message = new
        {
            Guid = guid,
            EventType = "ListingCreated"
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            "",
            _queueName,
            null,
            body
        );
    }
}