using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using service.interfaces;

public class RabbitPublisher : IDisposable, IRabbitPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly object _lock = new();

    public RabbitPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.BasicQos(0, 100, false);
    }

    public Task PublishAsync<T>(T message, string queueName)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            lock (_lock) // IMPORTANT: makes channel thread-safe
            {
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: props,
                    body: body);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // IMPORTANT: bubble up so OUTBOX worker can retry
            throw new Exception("Rabbit publish failed", ex);
        }
    }

    public void Dispose()
    {
        try
        {
            if (_channel.IsOpen)
                _channel.Close();

            _channel.Dispose();

            if (_connection.IsOpen)
                _connection.Close();

            _connection.Dispose();
        }
        catch
        {
            // ignore dispose errors
        }
    }
}