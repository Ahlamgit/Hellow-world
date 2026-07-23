using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class PaymentAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentAttemptRepository(ApplicationDbContext context) => _context = context;

    public Task<BookingPaymentAttempt?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _context.BookingPaymentAttempts
            .Include(a => a.BookingPayment)
            .FirstOrDefaultAsync(a => a.SessionId == sessionId || a.ProviderTransactionId == sessionId, cancellationToken);

    public Task<BookingPaymentAttempt?> GetActiveAttemptAsync(Guid bookingPaymentId, CancellationToken cancellationToken = default) =>
        _context.BookingPaymentAttempts
            .Where(a => a.BookingPaymentId == bookingPaymentId &&
                        (a.Status == PaymentAttemptStatus.Pending || a.Status == PaymentAttemptStatus.Processing))
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> HasCompletedAttemptAsync(Guid bookingPaymentId, CancellationToken cancellationToken = default) =>
        _context.BookingPaymentAttempts.AnyAsync(
            a => a.BookingPaymentId == bookingPaymentId && a.Status == PaymentAttemptStatus.Completed,
            cancellationToken);

    public Task<bool> WebhookEventExistsAsync(string provider, string webhookEventId, CancellationToken cancellationToken = default) =>
        _context.PaymentWebhookEvents.AnyAsync(
            e => e.Provider == provider && e.WebhookEventId == webhookEventId,
            cancellationToken);

    public async Task AddAttemptAsync(BookingPaymentAttempt attempt, CancellationToken cancellationToken = default) =>
        await _context.BookingPaymentAttempts.AddAsync(attempt, cancellationToken);

    public async Task AddWebhookEventAsync(PaymentWebhookEvent webhookEvent, CancellationToken cancellationToken = default) =>
        await _context.PaymentWebhookEvents.AddAsync(webhookEvent, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
