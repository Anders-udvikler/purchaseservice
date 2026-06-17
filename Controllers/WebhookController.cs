using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using service.interfaces;

[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{

    private IRabbitPublisher _publisher;

    private ILogger<WebhookController> _logger;
    public WebhookController(IRabbitPublisher publisher,ILogger<WebhookController> logger)
    {
        _publisher=publisher;
        _logger=logger;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        DotNetEnv.Env.Load();
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        var secret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");

        if (string.IsNullOrEmpty(secret))
        {
            _logger.LogInformation("Webhook secret is missing");
            return BadRequest("Webhook secret not configured");
        }

        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogInformation("Missing Stripe-Signature header");
            return BadRequest("Missing Stripe-Signature header");
        }

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                secret
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                if (session == null)
                {
                    _logger.LogInformation("Session is null");
                    return BadRequest();
                }

                var productId = session.Metadata.ContainsKey("productId")
                    ? session.Metadata["productId"]
                    : "unknown";

                var quantity = session.Metadata.ContainsKey("quantity")
                    ? int.Parse(session.Metadata["quantity"])
                    : 0;
                var message = new
                {
                    eventType = "PurchaseCompleted",
                    guid = productId,
                    quantity = quantity
                };
                await _publisher.PublishAsync(message,"");

                _logger.LogInformation("Message sent to RabbitMQ");
            }
            if(stripeEvent.Type == "checkout.session.failed")
            {
                                var session = stripeEvent.Data.Object as Session;

                if (session == null)
                {
                    _logger.LogInformation("Session is null");
                    return BadRequest();
                }

                var productId = session.Metadata.ContainsKey("productId")
                    ? session.Metadata["productId"]
                    : "unknown";

                var quantity = session.Metadata.ContainsKey("quantity")
                    ? int.Parse(session.Metadata["quantity"])
                    : 0;
                var message = new
                {
                    eventType = "PurchaseFailed",
                    guid = productId,
                    quantity = quantity
                };
                await _publisher.PublishAsync(message,"");
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError("Stripe error: " + ex.Message);
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError("General error: " + ex.Message);
            return BadRequest();
        }
    }
}