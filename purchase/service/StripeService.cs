using Stripe;
using Stripe.Checkout;
using DotNetEnv;
using Purchase.Models;
using service.interfaces;
using models;

public class StripeService
{

    public StripeService()
    {
        DotNetEnv.Env.Load();
        var key = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        if (string.IsNullOrEmpty(key))
            throw new Exception("STRIPE_SECRET_KEY not set");

        StripeConfiguration.ApiKey = key;
    }

public string CreateCheckoutSession(Order order,string eventid)
{
    var lineItems = order.OrderItems.Select(item =>
        new SessionLineItemOptions
        {
            Quantity = item.Quantity,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "dkk",

                // convert kroner → øre
                UnitAmount = (long)(item.Price * 100),

                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.Title
                }
            }
        }).ToList();

    var options = new SessionCreateOptions
    {
        Mode = "payment",

        SuccessUrl = "http://localhost:5258/success",
        CancelUrl = "http://localhost:5258/cancel",

        LineItems = lineItems,

        Metadata = new Dictionary<string, string>
        {
            {"eventid",eventid},
            { "orderid", order.Id ?? string.Empty },
            { "totalQuantity", order.OrderItems.Sum(x => x.Quantity).ToString() }
        }
    };

    var service = new SessionService();
    var session = service.Create(options);

    return session.Url;
}

public string CreateCheckoutSession(Order order)
{
    var lineItems = order.OrderItems.Select(item =>
        new SessionLineItemOptions
        {
            Quantity = item.Quantity,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "dkk",

                // convert kroner → øre
                UnitAmount = (long)(item.Price * 100),

                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.Title
                }
            }
        }).ToList();

    var options = new SessionCreateOptions
    {
        Mode = "payment",

        SuccessUrl = "http://localhost:5258/success",
        CancelUrl = "http://localhost:5258/cancel",

        LineItems = lineItems,

        Metadata = new Dictionary<string, string>
        {
            { "orderid", order.Id ?? string.Empty },
            { "totalQuantity", order.OrderItems.Sum(x => x.Quantity).ToString() }
        }
    };

    var service = new SessionService();
    var session = service.Create(options);

    return session.Url;
}
    
}