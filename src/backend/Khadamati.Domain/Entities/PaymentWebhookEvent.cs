using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class PaymentWebhookEvent : BaseEntity
{
    public string Provider { get; set; } = string.Empty;
    public string WebhookEventId { get; set; } = string.Empty;
    public Guid? BookingPaymentAttemptId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? EventStatus { get; set; }

    public BookingPaymentAttempt? BookingPaymentAttempt { get; set; }
}
