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
    private readonly EventEnvelopeService<Order> _envelopeService;
    private readonly OrderService _orderService;

    public WebhookController(
        ILogger<WebhookController> logger,
        EventEnvelopeService<Order> envelopeService,
        OrderService orderService)
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

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                secret
            );

            // =====================================================
            // SUCCESS EVENT
            // =====================================================
            if (stripeEvent.Type == "checkout.session.completed")
            {
                if (stripeEvent.Data.Object is not Session session)
                    return BadRequest("Invalid session payload");

                if (!session.Metadata.TryGetValue("orderId", out var orderId) ||
                    !session.Metadata.TryGetValue("eventId", out var eventId))
                {
                    return BadRequest("Missing metadata");
                }

                var envelope = await _envelopeService.GetEventById(eventId);

                if (envelope == null)
                    return BadRequest("Event not found");

                envelope.payload.OrderStatus = OrderStatus.Completed;

                await _orderService.UpdateOrder(envelope.payload);

                var purchaseCompletedEvent = new EventEnvelope<Order>
                {
                    eventId = Guid.NewGuid().ToString(),
                    eventType = "PurchaseCompleted",
                    eventVersion = 1,
                    occurredAt = DateTime.UtcNow,
                    producer = "PurchaseService",
                    correlationId = eventId,
                    causationId = stripeEvent.Id,
                    payload = envelope.payload,
                    published = false
                };

                await _envelopeService.Addevent(purchaseCompletedEvent);

                _logger.LogInformation("PurchaseCompleted processed for Order {OrderId}", orderId);

                return Ok();
            }

            // =====================================================
            // FAILURE EVENT
            // =====================================================
            if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                if (stripeEvent.Data.Object is not PaymentIntent intent)
                    return BadRequest("Invalid payment intent");

                if (!intent.Metadata.TryGetValue("orderId", out var orderId))
                    return BadRequest("Missing orderId");

                var envelope = await _envelopeService.GetEventById(orderId);

                if (envelope == null)
                    return BadRequest("Event not found");

                envelope.payload.OrderStatus = OrderStatus.Cancelled;

                await _orderService.UpdateOrder(envelope.payload);

                var purchaseFailedEvent = new EventEnvelope<Order>
                {
                    eventId = Guid.NewGuid().ToString(),
                    eventType = "PurchaseFailed",
                    eventVersion = 1,
                    occurredAt = DateTime.UtcNow,
                    producer = "PurchaseService",
                    correlationId = orderId,
                    causationId = stripeEvent.Id,
                    payload = envelope.payload,
                    published = false
                };

                await _envelopeService.Addevent(purchaseFailedEvent);

                _logger.LogInformation("PurchaseFailed processed for Order {OrderId}", orderId);

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