using Microsoft.AspNetCore.Mvc;
using Purchase.Models;
using service.interfaces;
using Stripe;

[ApiController]
[Route("purchase")]
public class PurchaseController : ControllerBase
{
    private readonly StripeService _stripe;

    public PurchaseController()
    {
        
        _stripe = new StripeService();
    }

    [HttpPost]
    public IActionResult CreatePurchase([FromBody] Order order)
    {
        var url = _stripe.CreateCheckoutSession(
            order
        );

        return Ok(new { checkoutUrl = url });
    }
}