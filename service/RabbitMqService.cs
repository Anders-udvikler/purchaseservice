using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using service.interfaces;

public class RabbitPublisher : IRabbitPublisher
{
    private readonly IConnection _connection;

    public RabbitPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            AutomaticRecoveryEnabled = true
        };

        _connection = factory.CreateConnection();
    }

    private IModel CreateChannel()
    {
        return _connection.CreateModel();
    }

    public Task PublishAsync<T>(T message, string queueName)
    {
        using var channel = CreateChannel();

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange: "",              // default exchange
            routingKey: queueName,    // MUST equal queue name
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}