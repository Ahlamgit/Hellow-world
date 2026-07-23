using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

/// <summary>
/// Business payment obligation for a booking (1:0..1).
/// Gateway interactions are tracked in <see cref="BookingPaymentAttempt"/>.
/// </summary>
public class BookingPayment : BaseEntity
{
    public Guid ServiceRequestId { get; set; }
    public Guid PayerUserId { get; set; }
    public Guid PayeeUserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "SAR";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? FailureReason { get; set; }

    /// <summary>Active/last payment provider name (Development, Moyasar, Areeba).</summary>
    public string PaymentProvider { get; set; } = "Development";

    /// <summary>Current/active attempt for checkout and webhook correlation.</summary>
    public Guid? CurrentAttemptId { get; set; }

    /// <summary>Denormalized from current attempt for fast lookups.</summary>
    public string? GatewaySessionId { get; set; }

    /// <summary>Denormalized from current attempt for fast lookups.</summary>
    public string? GatewayTransactionId { get; set; }

    public DateTime? FailedAt { get; set; }

    public ServiceRequest ServiceRequest { get; set; } = null!;
    public User Payer { get; set; } = null!;
    public User Payee { get; set; } = null!;
    public BookingPaymentAttempt? CurrentAttempt { get; set; }
    public ICollection<BookingPaymentAttempt> Attempts { get; set; } = new List<BookingPaymentAttempt>();
}
