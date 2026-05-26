namespace service.interfaces
{
    public interface IRabbitPublisher
    {
    Task PublishAsync<T>(T message, string routingKey);
    }
}


