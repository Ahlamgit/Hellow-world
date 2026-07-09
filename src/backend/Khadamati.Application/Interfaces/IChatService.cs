using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Messaging;

namespace Khadamati.Application.Interfaces;

public interface IChatService
{
    Task<IReadOnlyList<ChatConversationDto>> GetMyConversationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ChatConversationDto> GetOrCreateBookingConversationAsync(Guid bookingId, Guid userId, string role, CancellationToken cancellationToken = default);
    Task<PagedResult<ChatMessageDto>> GetMessagesAsync(Guid conversationId, Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ChatMessageDto> SendMessageAsync(Guid conversationId, Guid userId, SendChatMessageDto request, CancellationToken cancellationToken = default);
}
