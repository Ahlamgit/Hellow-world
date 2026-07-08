using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Features.Subscriptions.Commands;
using Khadamati.Application.Features.Subscriptions.Queries;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Current user's subscription — view, subscribe, cancel, auto-renew.</summary>
[ApiController]
[Route("api/v1/me")]
[Authorize]
[Produces("application/json")]
public class MeSubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public MeSubscriptionController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("subscription")]
    [SwaggerOperation(Summary = "Get current active subscription")]
    [ProducesResponseType(typeof(ApiResponse<UserSubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrent(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new GetCurrentUserSubscriptionQuery(userId), ct);
        return Ok(ApiResponse<UserSubscriptionDto?>.Ok(result));
    }

    [HttpGet("subscriptions")]
    [SwaggerOperation(Summary = "List subscription history")]
    public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new GetUserSubscriptionHistoryQuery(userId, page, pageSize), ct);
        return Ok(ApiResponse<PagedResult<UserSubscriptionDto>>.Ok(result));
    }

    [HttpPost("subscription")]
    [Authorize(Roles = "Craftsman,StoreOwner")]
    [SwaggerOperation(Summary = "Subscribe to a plan")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequestDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new SubscribeCommand(userId, request), ct);
        return CreatedAtAction(nameof(GetCurrent), ApiResponse<UserSubscriptionDto>.Ok(result, "Subscription created successfully."));
    }

    [HttpPost("subscription/{id:guid}/cancel")]
    [SwaggerOperation(Summary = "Cancel a subscription")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelSubscriptionDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new CancelUserSubscriptionCommand(userId, id, request), ct);
        return Ok(ApiResponse<SubscriptionActionResponseDto>.Ok(result));
    }

    [HttpPatch("subscription/auto-renew")]
    [SwaggerOperation(Summary = "Toggle auto-renew on active subscription")]
    public async Task<IActionResult> UpdateAutoRenew([FromBody] UpdateAutoRenewDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new UpdateAutoRenewCommand(userId, request), ct);
        return Ok(ApiResponse<UserSubscriptionDto>.Ok(result, "Auto-renew updated."));
    }
}
