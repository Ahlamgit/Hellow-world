using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class BookingSlotReservation : BaseEntity
{
    public Guid CraftsmanId { get; set; }
    public Guid ServiceRequestId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsActive { get; set; } = true;

    public User Craftsman { get; set; } = null!;
    public ServiceRequest ServiceRequest { get; set; } = null!;
}
