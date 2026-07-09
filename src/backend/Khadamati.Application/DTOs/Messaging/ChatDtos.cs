namespace Khadamati.Application.DTOs.Messaging;

public class ChatConversationDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid CraftsmanId { get; set; }
    public string CraftsmanName { get; set; } = string.Empty;
    public DateTime? LastMessageAt { get; set; }
    public string? LastMessagePreview { get; set; }
    public int UnreadCount { get; set; }
}

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsMine { get; set; }
}

public class SendChatMessageDto
{
    public string Body { get; set; } = string.Empty;
}
