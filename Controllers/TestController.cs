using DTO;
using Microsoft.AspNetCore.Mvc;
using models;
using mutation;
using Purchase.Models;
using RabbitMQ.Client;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    public RabbitPublisher _publisher { get; set; }
    public TestController(RabbitPublisher publisher)
    {
        _publisher=publisher;
    }

    [HttpPost("purchase")]
    public IActionResult TestPurchase([FromBody] OrderCreated request)
    {
        if(request.UserId==null)
        {
            
        }
        Order order =new Order
        {
            Id=request.OrderId,
            UserGuid=request.UserId,
            OrderStatus=Purchase.Enums.OrderStatus.Pending,
        };
        new EventEnvelope<Order>
        {
            eventId= Guid.NewGuid().ToString(),
            eventType= "created",
            eventVersion=1,
            occurredAt= new DateTime().Date,
            producer="purchase-service",
            correlationId= "0",
            payload=order
        };
        var message = new
        {
            eventType = "PurchaseCompleted",
            guid = request.OrderId,
            User = request.UserId,
            Post = request.SalesPostGuid
        };
        _publisher.PublishAsync(message,"product_storage");
        return Ok("Test message sent");
    }
}

public class TestPurchaseRequest
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}