using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface IPaymentGateway
{
    string ProviderName { get; }
    bool SupportsClientSideConfirmation { get; }
    Task<PaymentSessionDto> CreateSessionAsync(PaymentSessionRequest request, CancellationToken cancellationToken = default);
    Task<PaymentVerificationResult> VerifyAsync(string sessionId, decimal expectedAmount, string currency, CancellationToken cancellationToken = default);
}
