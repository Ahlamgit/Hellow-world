using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

/// <summary>
/// A single gateway interaction under a booking payment obligation.
/// One BookingPayment may have many attempts; only one may be Completed.
/// </summary>
public class BookingPaymentAttempt : BaseEntity
{
    public Guid BookingPaymentId { get; set; }
    public string PaymentProvider { get; set; } = "Development";
    public int AttemptNumber { get; set; }
    public string? GatewaySessionId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "SAR";
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    public DateTime? FailedDate { get; set; }
    public string? FailureReason { get; set; }
    public string? WebhookEventId { get; set; }

    public BookingPayment BookingPayment { get; set; } = null!;
}
