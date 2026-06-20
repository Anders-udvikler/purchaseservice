using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PurchaseService.Tests.Controllers;

public class TestControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TestControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Should_Return_Success()
    {
        // arrange
        var json = """
        {
            "userId": "test-user",
            "productId": "test-product",
            "amount": 100
        }
        """;

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // act
        var response = await _client.PostAsync("/api/test", content);

        var body = await response.Content.ReadAsStringAsync();

        // assert (IMPORTANT: shows real error if it fails)
        Assert.True(response.IsSuccessStatusCode, body);
    }
}