using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class ServiceRequest : BaseEntity
{
    public string BookingReference { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid CraftsmanId { get; set; }
    public Guid? AddressId { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime SlotEnd { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? PaymentDueAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public string? CancellationReason { get; set; }
    public Guid? RescheduledFromId { get; set; }
    public int? CustomerRating { get; set; }
    public string? CustomerReview { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public User Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public User Craftsman { get; set; } = null!;
    public Address? Address { get; set; }
    public ServiceRequest? RescheduledFrom { get; set; }
    public BookingPayment? Payment { get; set; }
    public BookingSlotReservation? SlotReservation { get; set; }
    public ICollection<ServiceRequestStatusHistory> StatusHistory { get; set; } = new List<ServiceRequestStatusHistory>();
}
