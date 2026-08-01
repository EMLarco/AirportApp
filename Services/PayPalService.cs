using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace AirportWebApp.Services;

public class PayPalService
{
    private readonly PayPalHttpClient _client;

    public PayPalService(string clientId, string secret, string mode = "sandbox")
    {
        var environment = mode.ToLower() == "sandbox"
            ? (PayPalEnvironment)new SandboxEnvironment(clientId, secret)
            : new LiveEnvironment(clientId, secret);
        _client = new PayPalHttpClient(environment);
    }

    public async Task<Order> CreateOrder(decimal amount, string returnUrl, string cancelUrl, string currency = "USD")
    {
        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("0.00")
                    }
                }
            },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl
            }
        });

        var response = await _client.Execute(request);
        return response.Result<Order>();
    }

    public async Task<Order> CaptureOrder(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest());
        var response = await _client.Execute(request);
        return response.Result<Order>();
    }
}
