using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using service.interfaces;

namespace PurchaseService.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IRabbitPublisher> RabbitMock { get; } = new();
    public Mock<StripeService> StripeMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove real RabbitMQ
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IRabbitPublisher));

            if (descriptor != null)
                services.Remove(descriptor);

            // Replace with mock
            services.AddSingleton(RabbitMock.Object);

            // OPTIONAL: mock Stripe if used
            var stripeDesc = services.SingleOrDefault(
                d => d.ServiceType == typeof(StripeService));

            if (stripeDesc != null)
                services.Remove(stripeDesc);

            services.AddSingleton(StripeMock.Object);
        });
    }
}