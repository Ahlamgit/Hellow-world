using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Features.Messaging;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Messaging;

public record RegisterPushTokenCommand(Guid UserId, RegisterPushTokenDto Request) : IRequest<Unit>;
public record UnregisterPushTokenCommand(Guid UserId, string Token) : IRequest<Unit>;
public record GetMyChatConversationsQuery(Guid UserId) : IRequest<IReadOnlyList<ChatConversationDto>>;
public record GetBookingChatConversationQuery(Guid BookingId, Guid UserId, string Role) : IRequest<ChatConversationDto>;
public record GetChatMessagesQuery(Guid ConversationId, Guid UserId, int Page, int PageSize) : IRequest<Common.PagedResult<ChatMessageDto>>;
public record SendChatMessageCommand(Guid ConversationId, Guid UserId, SendChatMessageDto Request) : IRequest<ChatMessageDto>;

public class RegisterPushTokenCommandHandler : IRequestHandler<RegisterPushTokenCommand, Unit>
{
    private readonly IDeviceTokenService _service;
    public RegisterPushTokenCommandHandler(IDeviceTokenService service) => _service = service;
    public async Task<Unit> Handle(RegisterPushTokenCommand request, CancellationToken ct)
    {
        await _service.RegisterAsync(request.UserId, request.Request, ct);
        return Unit.Value;
    }
}

public class UnregisterPushTokenCommandHandler : IRequestHandler<UnregisterPushTokenCommand, Unit>
{
    private readonly IDeviceTokenService _service;
    public UnregisterPushTokenCommandHandler(IDeviceTokenService service) => _service = service;
    public async Task<Unit> Handle(UnregisterPushTokenCommand request, CancellationToken ct)
    {
        await _service.UnregisterAsync(request.UserId, request.Token, ct);
        return Unit.Value;
    }
}

public class GetMyChatConversationsQueryHandler : IRequestHandler<GetMyChatConversationsQuery, IReadOnlyList<ChatConversationDto>>
{
    private readonly IChatService _service;
    public GetMyChatConversationsQueryHandler(IChatService service) => _service = service;
    public Task<IReadOnlyList<ChatConversationDto>> Handle(GetMyChatConversationsQuery request, CancellationToken ct) =>
        _service.GetMyConversationsAsync(request.UserId, ct);
}

public class GetBookingChatConversationQueryHandler : IRequestHandler<GetBookingChatConversationQuery, ChatConversationDto>
{
    private readonly IChatService _service;
    public GetBookingChatConversationQueryHandler(IChatService service) => _service = service;
    public Task<ChatConversationDto> Handle(GetBookingChatConversationQuery request, CancellationToken ct) =>
        _service.GetOrCreateBookingConversationAsync(request.BookingId, request.UserId, request.Role, ct);
}

public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, Common.PagedResult<ChatMessageDto>>
{
    private readonly IChatService _service;
    public GetChatMessagesQueryHandler(IChatService service) => _service = service;
    public Task<Common.PagedResult<ChatMessageDto>> Handle(GetChatMessagesQuery request, CancellationToken ct) =>
        _service.GetMessagesAsync(request.ConversationId, request.UserId, request.Page, request.PageSize, ct);
}

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, ChatMessageDto>
{
    private readonly IChatService _service;
    public SendChatMessageCommandHandler(IChatService service) => _service = service;
    public Task<ChatMessageDto> Handle(SendChatMessageCommand request, CancellationToken ct) =>
        _service.SendMessageAsync(request.ConversationId, request.UserId, request.Request, ct);
}
