using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }

    public ChatConversation Conversation { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
