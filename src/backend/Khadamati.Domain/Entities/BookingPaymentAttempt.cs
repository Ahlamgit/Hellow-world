using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class BookingPaymentAttempt : BaseEntity
{
    public Guid BookingPaymentId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string? ProviderTransactionId { get; set; }
    public string? ProviderUuid { get; set; }
    public string MerchantTransactionId { get; set; } = string.Empty;
    public string? TransactionToken { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentAttemptStatus Status { get; set; } = PaymentAttemptStatus.Pending;
    public string? GatewayStatus { get; set; }
    public string? ReturnType { get; set; }
    public string? RedirectUrl { get; set; }
    public string? ThreeDSReference { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? FailureReason { get; set; }
    public string? RetryReason { get; set; }
    public string? WebhookEventId { get; set; }
    public int RetryCount { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }

    public BookingPayment BookingPayment { get; set; } = null!;
}
