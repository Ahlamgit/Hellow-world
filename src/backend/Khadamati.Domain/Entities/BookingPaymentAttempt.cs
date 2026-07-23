using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class BookingPaymentAttempt : BaseEntity
{
    public Guid BookingPaymentId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string? ProviderTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentAttemptStatus Status { get; set; } = PaymentAttemptStatus.Pending;
    public string? CheckoutUrl { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? CompletedAt { get; set; }

    public BookingPayment BookingPayment { get; set; } = null!;
}
