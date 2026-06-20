using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Text;
using service;
using Purchase.Models;
using models;
using Purchase.Enums;
using service.Grapql;

[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly IEventEnvelopeService<Order> _envelopeService;
    private readonly IOrderService _orderService;

    public WebhookController(
        ILogger<WebhookController> logger,
        IEventEnvelopeService<Order> envelopeService,
        IOrderService orderService)
    {
        _logger = logger;
        _envelopeService = envelopeService;
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        try
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            var secret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
            if (string.IsNullOrWhiteSpace(secret))
                return BadRequest("Webhook secret not configured");

            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(signature))
                return BadRequest("Missing Stripe-Signature header");

            var stripeEvent = EventUtility.ConstructEvent(json, signature, secret);

            // =====================================================
            // SUCCESS: Checkout completed
            // =====================================================
            if (stripeEvent.Type == "checkout.session.completed")
            {
                if (stripeEvent.Data.Object is not Session session)
                    return BadRequest("Invalid session payload");

                if (!session.Metadata.TryGetValue("orderId", out var orderId))
                    return BadRequest("Missing orderId metadata");

                // FIX 1: Use OrderService (source of truth)
                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                    return NotFound("Order not found");

                // FIX 2: Idempotency guard
                if (order.OrderStatus == OrderStatus.Completed)
                    return Ok();

                order.OrderStatus = OrderStatus.Completed;

                await _orderService.UpdateOrder(order);

                // FIX 3: Correct event naming
                var envelope = new EventEnvelope<Order>
                {
                    eventId = Guid.NewGuid().ToString(),
                    eventType = "OrderCompleted",
                    eventVersion = 1,
                    occurredAt = DateTime.UtcNow,
                    producer = "PurchaseService",
                    correlationId = orderId,
                    causationId = stripeEvent.Id,
                    payload = order,
                    published = false
                };

                await _envelopeService.AddEvent(envelope);

                _logger.LogInformation("OrderCompleted processed for {OrderId}", orderId);

                return Ok();
            }

            // =====================================================
            // FAILURE: Payment failed
            // =====================================================
            if (stripeEvent.Type == "payment_intent.payment_failed" || stripeEvent.Type== "checkout.session.expired")
            {
                if (stripeEvent.Data.Object is not PaymentIntent intent)
                    return BadRequest("Invalid payment intent");

                if (!intent.Metadata.TryGetValue("orderId", out var orderId))
                    return BadRequest("Missing orderId metadata");

                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                    return NotFound("Order not found");

                // FIX 4: Idempotency guard
                if (order.OrderStatus == OrderStatus.Cancelled)
                    return Ok();

                order.OrderStatus = OrderStatus.Cancelled;

                await _orderService.UpdateOrder(order);

                var envelope = new EventEnvelope<Order>
                {
                    eventId = Guid.NewGuid().ToString(),
                    eventType = "OrderFailed",
                    eventVersion = 1,
                    occurredAt = DateTime.UtcNow,
                    producer = "PurchaseService",
                    correlationId = orderId,
                    causationId = stripeEvent.Id,
                    payload = order,
                    published = false
                };

                await _envelopeService.AddEvent(envelope);

                _logger.LogInformation("OrderFailed processed for {OrderId}", orderId);

                return Ok();
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected webhook error");
            return BadRequest();
        }
    }
}
