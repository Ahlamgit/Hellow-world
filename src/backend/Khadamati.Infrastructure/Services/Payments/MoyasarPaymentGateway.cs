using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Payments;

/// <summary>
/// Moyasar payment gateway — creates hosted invoices and verifies payment status via REST API.
/// Falls back to development sessions when SecretKey is not configured.
/// </summary>
public class MoyasarPaymentGateway : IPaymentGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MoyasarPaymentGateway> _logger;
    private readonly DevelopmentPaymentGateway _fallback;

    public MoyasarPaymentGateway(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<MoyasarPaymentGateway> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _fallback = new DevelopmentPaymentGateway(configuration);
    }

    public string ProviderName => "Moyasar";

    private bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_configuration["Payment:Moyasar:SecretKey"]);

    private string ApiBaseUrl =>
        _configuration["Payment:Moyasar:ApiBaseUrl"]?.TrimEnd('/') ?? "https://api.moyasar.com/v1";

    public async Task<PaymentSessionDto> CreateSessionAsync(
        PaymentSessionRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("Moyasar secret key not configured — using development checkout session.");
            return await _fallback.CreateSessionAsync(request, cancellationToken);
        }

        var devSessionId = $"KHD-{request.PaymentId:N}";
        var callbackUrl = _configuration["Payment:Moyasar:CallbackUrl"]
            ?? $"{_configuration["App:WebBaseUrl"]?.TrimEnd('/')}/api/v1/webhooks/moyasar";
        var successUrl = _configuration["Payment:Moyasar:SuccessUrl"]
            ?? $"{_configuration["Payment:CheckoutBaseUrl"]}?session={devSessionId}&status=success";

        var payload = new
        {
            amount = ToHalalas(request.Amount),
            currency = request.Currency,
            description = string.IsNullOrWhiteSpace(request.Description) ? "Khadamati booking payment" : request.Description,
            callback_url = callbackUrl,
            success_url = successUrl,
            metadata = new Dictionary<string, string>
            {
                ["payment_id"] = request.PaymentId.ToString(),
                ["session_id"] = devSessionId,
            },
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{ApiBaseUrl}/invoices")
        {
            Content = JsonContent.Create(payload, options: JsonOptions),
        };
        ApplyBasicAuth(httpRequest);

        var client = _httpClientFactory.CreateClient(nameof(MoyasarPaymentGateway));
        var response = await client.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Moyasar invoice creation failed ({Status}): {Body}", response.StatusCode, body);
            throw new Application.Common.ApplicationException("Failed to create Moyasar payment session.");
        }

        var invoice = JsonSerializer.Deserialize<MoyasarInvoiceResponse>(body, JsonOptions)
            ?? throw new Application.Common.ApplicationException("Invalid Moyasar response.");

        if (string.IsNullOrWhiteSpace(invoice.Id) || string.IsNullOrWhiteSpace(invoice.Url))
            throw new Application.Common.ApplicationException("Moyasar response missing invoice id or url.");

        _logger.LogInformation("Moyasar invoice {InvoiceId} created for payment {PaymentId}", invoice.Id, request.PaymentId);

        return new PaymentSessionDto
        {
            SessionId = invoice.Id,
            CheckoutUrl = invoice.Url,
            Provider = ProviderName,
        };
    }

    public async Task<PaymentVerificationResult> VerifyAsync(
        string sessionId, decimal expectedAmount, string currency, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || sessionId.StartsWith("KHD-", StringComparison.OrdinalIgnoreCase))
            return await _fallback.VerifyAsync(sessionId, expectedAmount, currency, cancellationToken);

        var invoice = await FetchInvoiceAsync(sessionId, cancellationToken);
        if (invoice is null)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Payment session not found at Moyasar.",
            };
        }

        var status = invoice.Status?.ToLowerInvariant() ?? string.Empty;
        var paid = status is "paid" or "captured";
        var amountOk = invoice.Amount == ToHalalas(expectedAmount);
        var currencyOk = string.Equals(invoice.Currency, currency, StringComparison.OrdinalIgnoreCase);

        if (!paid || !amountOk || !currencyOk)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = paid ? "Payment amount or currency mismatch." : $"Payment status is {invoice.Status}.",
            };
        }

        return new PaymentVerificationResult
        {
            IsSuccessful = true,
            TransactionReference = invoice.Id ?? sessionId,
        };
    }

    private async Task<MoyasarInvoiceResponse?> FetchInvoiceAsync(string invoiceId, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/invoices/{invoiceId}");
        ApplyBasicAuth(httpRequest);

        var client = _httpClientFactory.CreateClient(nameof(MoyasarPaymentGateway));
        var response = await client.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MoyasarInvoiceResponse>(body, JsonOptions);
    }

    private void ApplyBasicAuth(HttpRequestMessage request)
    {
        var secret = _configuration["Payment:Moyasar:SecretKey"]!;
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{secret}:"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
    }

    private static int ToHalalas(decimal amount) => (int)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);

    private sealed class MoyasarInvoiceResponse
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public int Amount { get; set; }
        public string? Currency { get; set; }
        public string? Url { get; set; }
    }
}
