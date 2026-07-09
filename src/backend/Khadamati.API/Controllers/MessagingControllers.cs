using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Features.Messaging;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/devices")]
[Authorize]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public DevicesController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost("push-token")]
    [SwaggerOperation(Summary = "Register a device push token (FCM/APNs)")]
    public async Task<IActionResult> RegisterPushToken([FromBody] RegisterPushTokenDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        await _mediator.Send(new RegisterPushTokenCommand(userId, request), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Push token registered."));
    }

    [HttpDelete("push-token")]
    [SwaggerOperation(Summary = "Unregister a device push token")]
    public async Task<IActionResult> UnregisterPushToken([FromQuery] string token, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        await _mediator.Send(new UnregisterPushTokenCommand(userId, token), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Push token removed."));
    }
}

[ApiController]
[Route("api/v1/chat")]
[Authorize]
[Produces("application/json")]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ChatController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("conversations")]
    [SwaggerOperation(Summary = "List my booking chat conversations")]
    public async Task<IActionResult> ListConversations(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new GetMyChatConversationsQuery(userId), ct);
        return Ok(ApiResponse<IReadOnlyList<ChatConversationDto>>.Ok(result));
    }

    [HttpGet("bookings/{bookingId:guid}")]
    [SwaggerOperation(Summary = "Get or create chat for a booking")]
    public async Task<IActionResult> GetBookingChat(Guid bookingId, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var role = _currentUser.Role ?? "Customer";
        var result = await _mediator.Send(new GetBookingChatConversationQuery(bookingId, userId, role), ct);
        return Ok(ApiResponse<ChatConversationDto>.Ok(result));
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    [SwaggerOperation(Summary = "List messages in a conversation")]
    public async Task<IActionResult> GetMessages(
        Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new GetChatMessagesQuery(conversationId, userId, page, pageSize), ct);
        return Ok(ApiResponse<PagedResult<ChatMessageDto>>.Ok(result));
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    [SwaggerOperation(Summary = "Send a chat message")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendChatMessageDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new SendChatMessageCommand(conversationId, userId, request), ct);
        return Ok(ApiResponse<ChatMessageDto>.Ok(result));
    }
}
