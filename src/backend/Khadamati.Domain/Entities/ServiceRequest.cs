using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class ServiceRequest : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid? CraftsmanId { get; set; }
    public Guid? AddressId { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public string? Description { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public string? Notes { get; set; }
    public int? CustomerRating { get; set; }
    public string? CustomerReview { get; set; }

    public User Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public User? Craftsman { get; set; }
    public Address? Address { get; set; }
}
