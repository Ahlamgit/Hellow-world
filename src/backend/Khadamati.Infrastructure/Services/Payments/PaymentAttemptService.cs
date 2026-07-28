using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Repositories;

namespace Khadamati.Infrastructure.Services.Payments;

public class PaymentAttemptService : IPaymentAttemptService
{
    private readonly PaymentAttemptRepository _repository;

    public PaymentAttemptService(PaymentAttemptRepository repository) => _repository = repository;

    public async Task<BookingPaymentAttempt> CreateAttemptAsync(
        Guid bookingPaymentId,
        string provider,
        string merchantTransactionId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.HasCompletedAttemptAsync(bookingPaymentId, cancellationToken))
            throw new Application.Common.ConflictException("A completed payment attempt already exists for this booking.");

        var attempt = new BookingPaymentAttempt
        {
            BookingPaymentId = bookingPaymentId,
            Provider = provider,
            MerchantTransactionId = string.IsNullOrWhiteSpace(merchantTransactionId)
                ? Guid.NewGuid().ToString("N")
                : merchantTransactionId,
            SessionId = string.IsNullOrWhiteSpace(merchantTransactionId)
                ? Guid.NewGuid().ToString("N")
                : merchantTransactionId,
            Amount = amount,
            Currency = currency,
            Status = PaymentAttemptStatus.Pending,
        };

        await _repository.AddAttemptAsync(attempt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return attempt;
    }

    public Task<BookingPaymentAttempt?> GetByIdAsync(Guid attemptId, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(attemptId, cancellationToken);

    public Task<BookingPaymentAttempt?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _repository.GetBySessionIdAsync(sessionId, cancellationToken);

    public Task<BookingPaymentAttempt?> GetByMerchantTransactionIdAsync(string merchantTransactionId, CancellationToken cancellationToken = default) =>
        _repository.GetByMerchantTransactionIdAsync(merchantTransactionId, cancellationToken);

    public Task<BookingPaymentAttempt?> GetByProviderUuidAsync(string providerUuid, CancellationToken cancellationToken = default) =>
        _repository.GetByProviderUuidAsync(providerUuid, cancellationToken);

    public Task<BookingPaymentAttempt?> GetActiveAttemptAsync(Guid bookingPaymentId, CancellationToken cancellationToken = default) =>
        _repository.GetActiveAttemptAsync(bookingPaymentId, cancellationToken);

    public Task<bool> HasCompletedAttemptAsync(Guid bookingPaymentId, CancellationToken cancellationToken = default) =>
        bookingPaymentId == Guid.Empty
            ? Task.FromResult(false)
            : _repository.HasCompletedAttemptAsync(bookingPaymentId, cancellationToken);

    public async Task<bool> TryRegisterWebhookEventAsync(
        string provider,
        string webhookEventId,
        Guid? bookingPaymentAttemptId,
        string? eventStatus,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.WebhookEventExistsAsync(provider, webhookEventId, cancellationToken))
            return false;

        await _repository.AddWebhookEventAsync(new PaymentWebhookEvent
        {
            Provider = provider,
            WebhookEventId = webhookEventId,
            BookingPaymentAttemptId = bookingPaymentAttemptId,
            ProcessedAt = DateTime.UtcNow,
            EventStatus = eventStatus,
        }, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task ApplyAuthorizationResultAsync(
        BookingPaymentAttempt attempt,
        PaymentAuthorizationResult result,
        string? transactionToken,
        CancellationToken cancellationToken = default)
    {
        if (attempt.Status == PaymentAttemptStatus.Completed)
            return;

        attempt.TransactionToken = transactionToken;
        attempt.ProviderUuid = result.ProviderUuid ?? attempt.ProviderUuid;
        attempt.GatewayStatus = result.GatewayStatus;
        attempt.ReturnType = result.ReturnType;
        attempt.RedirectUrl = result.RedirectUrl;
        attempt.SessionId = result.ProviderUuid ?? attempt.SessionId;

        if (!result.IsAccepted)
        {
            attempt.Status = PaymentAttemptStatus.Failed;
            attempt.FailureReason = result.FailureReason;
            attempt.FailedAt = DateTime.UtcNow;
        }
        else if (string.Equals(result.ReturnType, "REDIRECT", StringComparison.OrdinalIgnoreCase))
        {
            attempt.Status = PaymentAttemptStatus.Processing;
            attempt.ThreeDSReference = result.RedirectUrl;
        }
        else
        {
            attempt.Status = PaymentAttemptStatus.AwaitingGatewayConfirmation;
        }

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAttemptCompletedAsync(
        BookingPaymentAttempt attempt,
        string? providerTransactionId,
        string? webhookEventId,
        CancellationToken cancellationToken = default)
    {
        if (attempt.Status == PaymentAttemptStatus.Completed)
            return;

        attempt.Status = PaymentAttemptStatus.Completed;
        attempt.ProviderTransactionId = providerTransactionId ?? attempt.ProviderTransactionId;
        attempt.ProviderUuid = providerTransactionId ?? attempt.ProviderUuid;
        attempt.WebhookEventId = webhookEventId ?? attempt.WebhookEventId;
        attempt.CompletedAt = DateTime.UtcNow;
        attempt.FailureReason = null;
        attempt.TransactionToken = null;
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAttemptFailedAsync(
        BookingPaymentAttempt attempt,
        string failureReason,
        CancellationToken cancellationToken = default)
    {
        if (attempt.Status == PaymentAttemptStatus.Completed)
            return;

        attempt.Status = PaymentAttemptStatus.Failed;
        attempt.FailureReason = failureReason;
        attempt.FailedAt = DateTime.UtcNow;
        attempt.TransactionToken = null;
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
