using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Khadamati.Infrastructure.Services.Payments;

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
    public bool RequiresClientAuthorizationHandoff => true;

    public Task<PaymentInitializationDto> InitialisePaymentAsync(
        PaymentInitializationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfiguredForInitialization())
            throw new Application.Common.ApplicationException("Areeba payment gateway is not configured.");

        return Task.FromResult(new PaymentInitializationDto
        {
            AttemptId = request.AttemptId,
            MerchantTransactionId = request.MerchantTransactionId,
            Provider = ProviderName,
            Amount = request.Amount,
            Currency = request.Currency,
            PublicIntegrationKey = _options.PublicIntegrationKey,
            PaymentJsScriptUrl = _options.PaymentJsScriptUrl,
            SessionId = request.MerchantTransactionId,
        });
    }

    public async Task<PaymentAuthorizationResult> AuthorizeAsync(
        PaymentAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfiguredForAuthorization())
        {
            return new PaymentAuthorizationResult
            {
                IsAccepted = false,
                FailureReason = "Areeba payment gateway is not configured.",
            };
        }

        if (string.IsNullOrWhiteSpace(request.TransactionToken))
        {
            return new PaymentAuthorizationResult
            {
                IsAccepted = false,
                FailureReason = "Transaction token is required.",
            };
        }

        var transactionMode = _options.TransactionMode.Equals("Preauthorize", StringComparison.OrdinalIgnoreCase)
            ? "preauthorize"
            : "debit";

        var payload = new Dictionary<string, object?>
        {
            ["merchantTransactionId"] = request.MerchantTransactionId,
            ["transactionToken"] = request.TransactionToken,
            ["amount"] = FormatAmount(request.Amount),
            ["currency"] = request.Currency,
            ["description"] = string.IsNullOrWhiteSpace(request.Description) ? "Khadamati booking payment" : request.Description,
            ["callbackUrl"] = _options.CallbackUrl,
            ["successUrl"] = _options.SuccessUrl,
            ["cancelUrl"] = _options.CancelUrl,
            ["errorUrl"] = _options.FailureUrl,
            ["customer"] = BuildCustomer(request),
        };

        var path = $"/api/v3/transaction/{_options.ApiKey}/{transactionMode}";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.ApiBaseUrl.TrimEnd('/')}{path}")
        {
            Content = JsonContent.Create(payload, options: JsonOptions),
        };
        ApplyBasicAuth(httpRequest);

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
            return new PaymentAuthorizationResult
            {
                IsAccepted = false,
                FailureReason = "Areeba authorization request timed out.",
            };
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var transaction = JsonSerializer.Deserialize<AreebaTransactionResponse>(body, JsonOptions);
        if (transaction is null)
        {
            return new PaymentAuthorizationResult
            {
                IsAccepted = false,
                FailureReason = "Invalid Areeba authorization response.",
            };
        }

        if (!response.IsSuccessStatusCode || transaction.Success == false)
        {
            var reason = transaction.Errors?.FirstOrDefault()?.ErrorMessage
                ?? transaction.Message
                ?? "Areeba authorization failed.";
            _logger.LogWarning("Areeba authorization failed ({Status}): {Body}", response.StatusCode, body);
            return new PaymentAuthorizationResult
            {
                IsAccepted = false,
                ProviderUuid = transaction.Uuid,
                GatewayStatus = transaction.ReturnType,
                ReturnType = transaction.ReturnType,
                FailureReason = reason,
            };
        }

        _logger.LogInformation(
            "Areeba authorization accepted for merchant transaction {MerchantTransactionId}, uuid {Uuid}, returnType {ReturnType}",
            request.MerchantTransactionId,
            transaction.Uuid,
            transaction.ReturnType);

        return new PaymentAuthorizationResult
        {
            IsAccepted = true,
            ProviderUuid = transaction.Uuid,
            GatewayStatus = transaction.ReturnType,
            ReturnType = transaction.ReturnType,
            RedirectUrl = transaction.RedirectUrl,
        };
    }

    public async Task<PaymentVerificationResult> VerifyAsync(
        string providerReference,
        decimal expectedAmount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfiguredForAuthorization())
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Areeba payment gateway is not configured.",
            };
        }

        if (string.IsNullOrWhiteSpace(providerReference))
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Invalid payment reference.",
            };
        }

        var transaction = await FetchStatusAsync(providerReference, cancellationToken);
        if (transaction is null)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Payment transaction not found at Areeba.",
            };
        }

        return MapVerification(transaction, expectedAmount, currency, providerReference);
    }

    internal static PaymentVerificationResult MapVerification(
        AreebaStatusResponse transaction,
        decimal expectedAmount,
        string currency,
        string providerReference)
    {
        var returnType = transaction.ReturnType?.ToUpperInvariant() ?? string.Empty;
        var result = transaction.Result?.ToUpperInvariant() ?? string.Empty;
        var paid = returnType is "FINISHED" || result is "OK" or "SUCCESS";
        var amountOk = ParseAmount(transaction.Amount) == expectedAmount;
        var currencyOk = string.Equals(transaction.Currency, currency, StringComparison.OrdinalIgnoreCase);

        if (!paid)
        {
            return new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = $"Payment status is {transaction.ReturnType ?? transaction.Result}.",
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
            TransactionReference = transaction.Uuid ?? providerReference,
        };
    }

    private async Task<AreebaStatusResponse?> FetchStatusAsync(string uuid, CancellationToken cancellationToken)
    {
        var path = $"/api/v3/status/{_options.ApiKey}/getByUuid/{Uri.EscapeDataString(uuid)}";
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_options.ApiBaseUrl.TrimEnd('/')}{path}");
        ApplyBasicAuth(httpRequest);

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
        return JsonSerializer.Deserialize<AreebaStatusResponse>(body, JsonOptions);
    }

    private static object BuildCustomer(PaymentAuthorizationRequest request)
    {
        var customer = new Dictionary<string, object?>
        {
            ["email"] = request.CustomerEmail,
        };

        if (!string.IsNullOrWhiteSpace(request.CustomerFirstName))
            customer["firstName"] = request.CustomerFirstName;
        if (!string.IsNullOrWhiteSpace(request.CustomerLastName))
            customer["lastName"] = request.CustomerLastName;

        return customer;
    }

    private void ApplyBasicAuth(HttpRequestMessage request)
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ApiUser}:{_options.ApiPassword}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
    }

    private bool IsConfiguredForInitialization() =>
        !string.IsNullOrWhiteSpace(_options.PublicIntegrationKey);

    private bool IsConfiguredForAuthorization() =>
        !string.IsNullOrWhiteSpace(_options.ApiKey) &&
        !string.IsNullOrWhiteSpace(_options.ApiUser) &&
        !string.IsNullOrWhiteSpace(_options.ApiPassword);

    private static string FormatAmount(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);

    private static decimal ParseAmount(string? amount) =>
        decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) ? parsed : 0m;

    internal sealed class AreebaTransactionResponse
    {
        public bool? Success { get; set; }
        public string? Uuid { get; set; }
        public string? PurchaseId { get; set; }
        public string? ReturnType { get; set; }
        public string? RedirectUrl { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Message { get; set; }
        public List<AreebaError>? Errors { get; set; }
    }

    internal sealed class AreebaStatusResponse
    {
        public string? Uuid { get; set; }
        public string? MerchantTransactionId { get; set; }
        public string? Result { get; set; }
        public string? ReturnType { get; set; }
        public string? Amount { get; set; }
        public string? Currency { get; set; }
        public string? TransactionType { get; set; }
    }

    internal sealed class AreebaError
    {
        public string? ErrorMessage { get; set; }
        public int? ErrorCode { get; set; }
    }
}
