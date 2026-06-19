using DTO;
using Microsoft.AspNetCore.Mvc;
using models;
using mutation;
using Purchase.Models;
using RabbitMQ.Client;
using service;
using service.Grapql;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    private RabbitPublisher _publisher { get; set; }
    private EventEnvelopeService<Order> EnvelopeService;
    private OrderService _orderservice;
    public TestController(RabbitPublisher publisher)
    {
        _publisher=publisher;
        EnvelopeService= new EventEnvelopeService<Order>();
        _orderservice = new OrderService();
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> TestPurchase([FromBody] OrderCreated request)
    {
        if(request.UserId==null)
        {
            
        }
        Order order =new Order
        {
            Id=request.OrderId,
            UserGuid=request.UserId,
            OrderStatus=Purchase.Enums.OrderStatus.Pending,
            email=request.Email

        };
         EventEnvelope<Order> envelope=new EventEnvelope<Order>
        {
            eventType= "created",
            eventVersion=1,
            occurredAt= new DateTime().Date,
            producer="purchase-service",
            correlationId= Request.Headers["My-Header"].ToString() ?? Guid.NewGuid().ToString(),
            payload=order
        };
        var message = new
        {
            eventType = "PurchaseCompleted",
            guid = request.OrderId,
            User = request.UserId,
            Post = request.SalesPostGuid
        };
        await _publisher.PublishAsync(message,"product_storage");
        await _orderservice.AddOrder(order);
        await EnvelopeService.Addevent(envelope);
        return Ok("Test message sent");
    }
}

