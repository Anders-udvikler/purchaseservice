using Microsoft.AspNetCore.Mvc;

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
    public IActionResult CreatePurchase([FromBody] PurchaseRequest request)
    {
        var url = _stripe.CreateCheckoutSession(
            request.ProductId,
            request.Quantity
        );

        return Ok(new { checkoutUrl = url });
    }
}

public class PurchaseRequest
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}