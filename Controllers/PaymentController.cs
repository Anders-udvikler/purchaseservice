using Microsoft.AspNetCore.Mvc;

[ApiController]
public class PaymentController : ControllerBase
{
    [HttpGet("/success")]
    public ContentResult Success()
    {
        var html = @"
        <html>
            <head>
                <title>Payment Success</title>
                <script>
                    setTimeout(function() {
                        window.location.href = '/swagger';
                    }, 10000);
                </script>
            </head>
            <body style='font-family: Arial; text-align: center; margin-top: 50px;'>
                <h1>Payment successful</h1>
                <p>You will be redirected shortly...</p>
            </body>
        </html>";

        return Content(html, "text/html");
    }

    [HttpGet("/cancel")]
    public ContentResult Cancel()
    {
        return Content("<h1>❌ Payment cancelled</h1>", "text/html");
    }
}