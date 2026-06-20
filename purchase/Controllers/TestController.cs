using DotNetEnv;
using DTO;
using Microsoft.AspNetCore.Mvc;
using models;
using mutation;
using Purchase.Models;
using RabbitMQ.Client;
using service;
using service.Grapql;
using service.interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IRabbitPublisher _publisher;
    private readonly IEventEnvelopeService<Order> _envelopeService;
    private readonly IOrderService _orderService;

    public TestController(
        IRabbitPublisher publisher,
        IEventEnvelopeService<Order> envelopeService,
        IOrderService orderService,
        ILogger<TestController> logger)
    {
        _publisher = publisher;
        _envelopeService = envelopeService;
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> TestPurchase([FromBody] OrderCreated request)
    {
        // FIX 1: proper validation
        if (request == null)
            return BadRequest("Request is null");

        if (string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest("UserId is required");

        if (string.IsNullOrWhiteSpace(request.OrderId))
            return BadRequest("OrderId is required");

        // FIX 2: build order properly
        var order = new Order
        {
            Id = request.OrderId,
            UserGuid = request.UserId,
            OrderStatus = Purchase.Enums.OrderStatus.Pending,
            email = request.Email
        };

        string guid= Guid.NewGuid().ToString();

        // FIX 4: event envelope correct naming
        var envelope = new EventEnvelope<Order>
        {
            eventId = guid,
            eventType = "OrderCreated",
            eventVersion = 1,
            occurredAt = DateTime.UtcNow,
            producer = "purchase-service",
            correlationId =guid,
            payload = order,
            published = false
        };

        // FIX 5: domain-first persistence order
        await _orderService.AddOrder(order);
        await _envelopeService.AddEvent(envelope);

        await _publisher.PublishAsync(envelope, "product_storage");

        _logger.LogInformation("OrderCreated test event sent for {OrderId}", request.OrderId);

        return Ok("Test purchase created");
    }
}

