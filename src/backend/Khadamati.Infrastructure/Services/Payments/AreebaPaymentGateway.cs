using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Payments;

/// <summary>
/// Areeba MPGS hosted checkout gateway. Business logic must not depend on this type —
/// resolve only via <see cref="IPaymentGateway"/>.
/// </summary>
public class AreebaPaymentGateway : IPaymentGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AreebaPaymentGateway> _logger;

    public AreebaPaymentGateway(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<AreebaPaymentGateway> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string ProviderName => "Areeba";

    private string MerchantId => _configuration["Payment:Areeba:MerchantId"]?.Trim() ?? string.Empty;
    private string ApiUsername => _configuration["Payment:Areeba:ApiUsername"]?.Trim()
        ?? (string.IsNullOrWhiteSpace(MerchantId) ? string.Empty : $"merchant.{MerchantId}");
    private string ApiPassword => _configuration["Payment:Areeba:ApiPassword"]?.Trim() ?? string.Empty;
    private string ApiBaseUrl => (_configuration["Payment:Areeba:ApiBaseUrl"]?.TrimEnd('/')
        ?? "https://epayment.areeba.com");
    private string ApiVersion => _configuration["Payment:Areeba:ApiVersion"]?.Trim() ?? "100";
    private string CheckoutJsBase => _configuration["Payment:Areeba:CheckoutBaseUrl"]?.TrimEnd('/')
        ?? ApiBaseUrl;

    private bool IsConfigured =>
        !string.IsNullOrWhiteSpace(MerchantId)
        && !string.IsNullOrWhiteSpace(ApiPassword);

    public async Task<PaymentSessionDto> CreateSessionAsync(
        PaymentSessionRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogError("Areeba is selected but MerchantId/ApiPassword are not configured.");
            throw new Application.Common.ApplicationException("Areeba payment gateway is not configured.");
        }

        var orderId = request.PaymentId.ToString("N");
        var successUrl = _configuration["Payment:Areeba:SuccessUrl"]
            ?? $"{_configuration["Payment:CheckoutBaseUrl"]}?session={orderId}&status=success";
        var cancelUrl = _configuration["Payment:Areeba:CancelUrl"]
            ?? $"{_configuration["Payment:CheckoutBaseUrl"]}?session={orderId}&status=cancel";

        var payload = new Dictionary<string, object?>
        {
            ["apiOperation"] = "INITIATE_CHECKOUT",
            ["interaction"] = new Dictionary<string, object?>
            {
                ["operation"] = "PURCHASE",
                ["returnUrl"] = successUrl,
                ["cancelUrl"] = cancelUrl,
                ["merchant"] = new Dictionary<string, object?>
                {
                    ["name"] = _configuration["Payment:Areeba:MerchantName"] ?? "Khadamati",
                },
            },
            ["order"] = new Dictionary<string, object?>
            {
                ["id"] = orderId,
                ["amount"] = request.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                ["currency"] = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency,
                ["description"] = string.IsNullOrWhiteSpace(request.Description)
                    ? "Khadamati booking payment"
                    : request.Description,
            },
            ["transaction"] = new Dictionary<string, object?>
            {
                ["source"] = "INTERNET",
            },
        };

        var url = $"{ApiBaseUrl}/api/rest/version/{ApiVersion}/merchant/{MerchantId}/session";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json"),
        };
        ApplyBasicAuth(httpRequest);

        var client = _httpClientFactory.CreateClient(nameof(AreebaPaymentGateway));
        var response = await client.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Areeba session creation failed ({Status})", response.StatusCode);
            throw new Application.Common.ApplicationException("Failed to create Areeba payment session.");
        }

        var sessionResponse = JsonSerializer.Deserialize<AreebaSessionResponse>(body, JsonOptions)
            ?? throw new Application.Common.ApplicationException("Invalid Areeba session response.");

        if (!string.Equals(sessionResponse.Result, "SUCCESS", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(sessionResponse.Session?.Id))
        {
            _logger.LogError("Areeba session response missing success/session id");
            throw new Application.Common.ApplicationException("Areeba session response was unsuccessful.");
        }

        // Bridge page runs checkout.js with the MPGS session id; order id is the stable verify key.
        var checkoutUrl = $"{CheckoutJsBase}/static/checkout/pay.html?session={Uri.EscapeDataString(sessionResponse.Session.Id)}"
            + $"&order={Uri.EscapeDataString(orderId)}";
        var configuredCheckout = _configuration["Payment:Areeba:HostedCheckoutUrl"];
        if (!string.IsNullOrWhiteSpace(configuredCheckout))
        {
            checkoutUrl = configuredCheckout
                .Replace("{sessionId}", Uri.EscapeDataString(sessionResponse.Session.Id), StringComparison.OrdinalIgnoreCase)
                .Replace("{orderId}", Uri.EscapeDataString(orderId), StringComparison.OrdinalIgnoreCase);
        }

        _logger.LogInformation(
            "Areeba checkout session created for payment {PaymentId} order {OrderId}",
            request.PaymentId, orderId);

        return new PaymentSessionDto
        {
            // Stable correlation id used for VerifyAsync / webhooks (MPGS order id).
            SessionId = orderId,
            GatewaySessionId = sessionResponse.Session.Id,
            CheckoutUrl = checkoutUrl,
            Provider = ProviderName,
        };
    }

    public async Task<PaymentVerificationResult> VerifyAsync(
        string sessionId, decimal expectedAmount, string currency, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Areeba payment gateway is not configured.",
            };
        }

        var order = await FetchOrderAsync(sessionId, cancellationToken);
        if (order is null)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Payment order not found at Areeba.",
            };
        }

        var status = order.Status?.ToUpperInvariant() ?? string.Empty;
        var paid = status is "CAPTURED" or "PAID" or "SUCCESS" or "AUTHORIZED";
        var amountOk = AmountMatches(order.Amount, expectedAmount);
        var currencyOk = string.Equals(order.Currency, currency, StringComparison.OrdinalIgnoreCase);

        if (!paid || !amountOk || !currencyOk)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = paid
                    ? "Payment amount or currency mismatch."
                    : $"Payment status is {order.Status}.",
            };
        }

        return new PaymentVerificationResult
        {
            IsSuccessful = true,
            TransactionReference = order.Id ?? sessionId,
        };
    }

    private async Task<AreebaOrderResponse?> FetchOrderAsync(string orderId, CancellationToken cancellationToken)
    {
        var url = $"{ApiBaseUrl}/api/rest/version/{ApiVersion}/merchant/{MerchantId}/order/{orderId}";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        ApplyBasicAuth(httpRequest);

        var client = _httpClientFactory.CreateClient(nameof(AreebaPaymentGateway));
        var response = await client.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<AreebaOrderResponse>(body, JsonOptions);
    }

    private void ApplyBasicAuth(HttpRequestMessage request)
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApiUsername}:{ApiPassword}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
    }

    private static bool AmountMatches(decimal? gatewayAmount, decimal expectedAmount)
    {
        if (gatewayAmount is null) return false;
        return Math.Abs(gatewayAmount.Value - expectedAmount) < 0.005m;
    }

    private sealed class AreebaSessionResponse
    {
        public string? Result { get; set; }
        public AreebaSessionPayload? Session { get; set; }
    }

    private sealed class AreebaSessionPayload
    {
        public string? Id { get; set; }
    }

    private sealed class AreebaOrderResponse
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Result { get; set; }
    }
}
