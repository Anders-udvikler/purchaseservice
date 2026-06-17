using Stripe;
using Stripe.Checkout;
using DotNetEnv;

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

    public string CreateCheckoutSession(string productId, int quantity)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = "http://localhost:5258/success",
            CancelUrl = "http://localhost:5258/cancel",

            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "dkk",
                        UnitAmount = 10000, // 100 kr (just test)
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Test product"
                        }
                    }
                }
            },

            Metadata = new Dictionary<string, string>
            {
                { "productId", productId },
                { "quantity", quantity.ToString() }
            }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return session.Url;
    }
}