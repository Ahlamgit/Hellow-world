using Khadamati.Application.DTOs.Payments;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;

namespace Khadamati.Application.Interfaces;

public interface IPaymentAttemptService
{
    Task<BookingPaymentAttempt> CreateAttemptAsync(
        Guid bookingPaymentId,
        string provider,
        PaymentSessionDto session,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);

    Task<BookingPaymentAttempt?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);

    Task<BookingPaymentAttempt?> GetActiveAttemptAsync(Guid bookingPaymentId, CancellationToken cancellationToken = default);

    Task<bool> HasCompletedAttemptAsync(Guid bookingPaymentId, CancellationToken cancellationToken = default);

    Task<bool> TryRegisterWebhookEventAsync(
        string provider,
        string webhookEventId,
        Guid? bookingPaymentAttemptId,
        string? eventStatus,
        CancellationToken cancellationToken = default);

    Task MarkAttemptCompletedAsync(
        BookingPaymentAttempt attempt,
        string? providerTransactionId,
        CancellationToken cancellationToken = default);

    Task MarkAttemptFailedAsync(
        BookingPaymentAttempt attempt,
        string failureReason,
        CancellationToken cancellationToken = default);
}
