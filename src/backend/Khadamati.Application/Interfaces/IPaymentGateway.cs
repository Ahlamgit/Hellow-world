using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface IPaymentGateway
{
    string ProviderName { get; }

    /// <summary>Whether the client may call ConfirmPayment to finalize the booking.</summary>
    bool SupportsClientSideConfirmation { get; }

    /// <summary>Whether the client must submit a provider token via AuthorizePayment after tokenization.</summary>
    bool RequiresClientAuthorizationHandoff { get; }

    Task<PaymentInitializationDto> InitialisePaymentAsync(
        PaymentInitializationRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentAuthorizationResult> AuthorizeAsync(
        PaymentAuthorizationRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentVerificationResult> VerifyAsync(
        string providerReference,
        decimal expectedAmount,
        string currency,
        CancellationToken cancellationToken = default);
}
