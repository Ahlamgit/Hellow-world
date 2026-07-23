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

    public Task<PaymentSessionDto> CreateSessionAsync(PaymentSessionRequest request, CancellationToken cancellationToken = default)
    {
        var sessionId = $"KHD-{request.PaymentId:N}";
        var baseUrl = _configuration["Payment:CheckoutBaseUrl"] ?? "http://localhost:5173/pay";
        var checkoutUrl = $"{baseUrl}?session={sessionId}&amount={request.Amount:F2}&currency={request.Currency}";

        return Task.FromResult(new PaymentSessionDto
        {
            SessionId = sessionId,
            CheckoutUrl = checkoutUrl,
            Provider = ProviderName,
        });
    }

    public Task<PaymentVerificationResult> VerifyAsync(string sessionId, decimal expectedAmount, string currency, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId) || !sessionId.StartsWith("KHD-", StringComparison.OrdinalIgnoreCase))
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
            TransactionReference = sessionId,
        });
    }
}
