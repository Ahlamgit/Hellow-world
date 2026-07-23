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
        PaymentSessionDto session,
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
            SessionId = session.SessionId,
            Amount = amount,
            Currency = currency,
            Status = PaymentAttemptStatus.Processing,
            CheckoutUrl = session.CheckoutUrl,
        };

        await _repository.AddAttemptAsync(attempt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return attempt;
    }

    public Task<BookingPaymentAttempt?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _repository.GetBySessionIdAsync(sessionId, cancellationToken);

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

    public async Task MarkAttemptCompletedAsync(
        BookingPaymentAttempt attempt,
        string? providerTransactionId,
        CancellationToken cancellationToken = default)
    {
        if (attempt.Status == PaymentAttemptStatus.Completed)
            return;

        attempt.Status = PaymentAttemptStatus.Completed;
        attempt.ProviderTransactionId = providerTransactionId ?? attempt.ProviderTransactionId;
        attempt.CompletedAt = DateTime.UtcNow;
        attempt.FailureReason = null;
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
        attempt.CompletedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
