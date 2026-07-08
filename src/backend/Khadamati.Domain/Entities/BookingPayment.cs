using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

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

    public ServiceRequest ServiceRequest { get; set; } = null!;
    public User Payer { get; set; } = null!;
    public User Payee { get; set; } = null!;
}
