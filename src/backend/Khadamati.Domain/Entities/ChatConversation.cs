using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class ChatConversation : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CraftsmanId { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ServiceRequest Booking { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
