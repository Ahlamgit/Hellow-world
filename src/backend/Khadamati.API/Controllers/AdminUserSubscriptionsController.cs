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

/// <summary>Admin management of user subscription records.</summary>
[ApiController]
[Route("api/v1/admin/user-subscriptions")]
[Authorize]
[Produces("application/json")]
public class AdminUserSubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminUserSubscriptionsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SubscriptionsView)]
    [SwaggerOperation(Summary = "Search user subscriptions")]
    public async Task<IActionResult> List([FromQuery] UserSubscriptionListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<UserSubscriptionDto>>.Ok(
            await _mediator.Send(new SearchAdminUserSubscriptionsQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SubscriptionsView)]
    [SwaggerOperation(Summary = "Get user subscription by ID")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<UserSubscriptionDto>.Ok(await _mediator.Send(new GetAdminUserSubscriptionByIdQuery(id), ct)));

    [HttpPost("users/{userId:guid}")]
    [HasPermission(PermissionCodes.SubscriptionsCreate)]
    [SwaggerOperation(Summary = "Manually grant a subscription to a user")]
    public async Task<IActionResult> Grant(Guid userId, [FromBody] AdminGrantSubscriptionDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId?.ToString();
        var result = await _mediator.Send(new GrantUserSubscriptionCommand(userId, request, adminId), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<UserSubscriptionDto>.Ok(result, "Subscription granted."));
    }

    [HttpPost("{id:guid}/cancel")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Cancel a user subscription (admin)")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelSubscriptionDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId?.ToString();
        var result = await _mediator.Send(new CancelAdminUserSubscriptionCommand(id, request, adminId), ct);
        return Ok(ApiResponse<SubscriptionActionResponseDto>.Ok(result));
    }
}
