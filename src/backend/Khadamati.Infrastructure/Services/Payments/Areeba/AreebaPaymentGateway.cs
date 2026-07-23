using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Khadamati.Infrastructure.Services.Payments.Areeba;

public class AreebaPaymentGateway : IPaymentGateway
{
    public const string ProviderNameConst = "Areeba";
    public const string HttpClientName = nameof(AreebaPaymentGateway);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AreebaOptions _options;
    private readonly ILogger<AreebaPaymentGateway> _logger;

    public AreebaPaymentGateway(
        IHttpClientFactory httpClientFactory,
        IOptions<AreebaOptions> options,
        ILogger<AreebaPaymentGateway> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public string ProviderName => ProviderNameConst;

    public bool SupportsClientSideConfirmation => false;

    private bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_options.SecretKey) &&
        !string.IsNullOrWhiteSpace(_options.MerchantId);

    public async Task<PaymentSessionDto> CreateSessionAsync(
        PaymentSessionRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
            throw new Application.Common.ApplicationException("Areeba payment gateway is not configured.");

        var payload = new
        {
            merchantId = _options.MerchantId,
            amount = request.Amount,
            currency = request.Currency,
            description = string.IsNullOrWhiteSpace(request.Description) ? "Khadamati booking payment" : request.Description,
            merchantReference = request.PaymentId.ToString(),
            callbackUrl = string.IsNullOrWhiteSpace(_options.CallbackUrl) ? request.CallbackUrl : _options.CallbackUrl,
            successUrl = _options.SuccessUrl,
            failureUrl = _options.FailureUrl,
            customerEmail = request.CustomerEmail,
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.ApiBaseUrl.TrimEnd('/')}/payment/sessions")
        {
            Content = JsonContent.Create(payload, options: JsonOptions),
        };
        ApplyAuth(httpRequest);

        var client = _httpClientFactory.CreateClient(HttpClientName);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(httpRequest, cts.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new Application.Common.ApplicationException("Areeba payment session request timed out.");
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Areeba session creation failed ({Status}): {Body}", response.StatusCode, body);
            throw new Application.Common.ApplicationException("Failed to create Areeba payment session.");
        }

        var session = JsonSerializer.Deserialize<AreebaSessionResponse>(body, JsonOptions)
            ?? throw new Application.Common.ApplicationException("Invalid Areeba session response.");

        if (string.IsNullOrWhiteSpace(session.SessionId) || string.IsNullOrWhiteSpace(session.CheckoutUrl))
            throw new Application.Common.ApplicationException("Areeba session response missing session id or checkout url.");

        _logger.LogInformation("Areeba session {SessionId} created for payment {PaymentId}", session.SessionId, request.PaymentId);

        return new PaymentSessionDto
        {
            SessionId = session.SessionId,
            CheckoutUrl = session.CheckoutUrl,
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

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Invalid payment session reference.",
            };
        }

        var transaction = await FetchTransactionAsync(sessionId, cancellationToken);
        if (transaction is null)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Payment session not found at Areeba.",
            };
        }

        return MapVerification(transaction, expectedAmount, currency, sessionId);
    }

    internal static PaymentVerificationResult MapVerification(
        AreebaTransactionResponse transaction, decimal expectedAmount, string currency, string sessionId)
    {
        var status = transaction.Status?.ToLowerInvariant() ?? string.Empty;
        var paid = status is "paid" or "captured" or "success" or "completed";
        var amountOk = transaction.Amount == expectedAmount;
        var currencyOk = string.Equals(transaction.Currency, currency, StringComparison.OrdinalIgnoreCase);

        if (!paid)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = $"Payment status is {transaction.Status}.",
            };
        }

        if (!amountOk || !currencyOk)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Payment amount or currency mismatch.",
            };
        }

        return new PaymentVerificationResult
        {
            IsSuccessful = true,
            TransactionReference = transaction.TransactionId ?? transaction.SessionId ?? sessionId,
        };
    }

    private async Task<AreebaTransactionResponse?> FetchTransactionAsync(string sessionId, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_options.ApiBaseUrl.TrimEnd('/')}/payment/transactions/{Uri.EscapeDataString(sessionId)}");
        ApplyAuth(httpRequest);

        var client = _httpClientFactory.CreateClient(HttpClientName);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(httpRequest, cts.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<AreebaTransactionResponse>(body, JsonOptions);
    }

    private void ApplyAuth(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            request.Headers.TryAddWithoutValidation("X-Api-Key", _options.ApiKey);

        if (!string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.SecretKey}:"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
        }
    }

    internal sealed class AreebaSessionResponse
    {
        public string? SessionId { get; set; }
        public string? CheckoutUrl { get; set; }
    }

    internal sealed class AreebaTransactionResponse
    {
        public string? SessionId { get; set; }
        public string? TransactionId { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }
}
