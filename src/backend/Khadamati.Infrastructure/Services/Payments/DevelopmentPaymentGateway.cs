using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Infrastructure.Services.Payments;

/// <summary>Development payment gateway — generates session IDs and validates KHD-prefixed references.</summary>
public class DevelopmentPaymentGateway : IPaymentGateway
{
    private readonly IConfiguration _configuration;

    public DevelopmentPaymentGateway(IConfiguration configuration) => _configuration = configuration;

    public string ProviderName => "Development";

    public bool SupportsClientSideConfirmation => true;

    public bool RequiresClientAuthorizationHandoff => false;

    public Task<PaymentInitializationDto> InitialisePaymentAsync(
        PaymentInitializationRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionId = $"KHD-{request.PaymentId:N}";
        var baseUrl = _configuration["Payment:CheckoutBaseUrl"] ?? "http://localhost:5173/pay";
        var checkoutUrl = $"{baseUrl}?session={sessionId}&amount={request.Amount:F2}&currency={request.Currency}";

        return Task.FromResult(new PaymentInitializationDto
        {
            AttemptId = request.AttemptId,
            MerchantTransactionId = request.MerchantTransactionId,
            Provider = ProviderName,
            Amount = request.Amount,
            Currency = request.Currency,
            SessionId = sessionId,
            CheckoutUrl = checkoutUrl,
        });
    }

    public Task<PaymentAuthorizationResult> AuthorizeAsync(
        PaymentAuthorizationRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Development gateway does not require authorization handoff.");

    public Task<PaymentVerificationResult> VerifyAsync(
        string providerReference,
        decimal expectedAmount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerReference) || !providerReference.StartsWith("KHD-", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new PaymentVerificationResult
            {
                IsSuccessful = false,
                FailureReason = "Invalid payment session reference.",
            });
        }

        return Task.FromResult(new PaymentVerificationResult
        {
            IsSuccessful = true,
            TransactionReference = providerReference,
        });
    }
}
