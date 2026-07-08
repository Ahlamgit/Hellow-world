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

/// <summary>
/// Administrator subscription plan management endpoints.
/// Supports full CRUD plus suspend, clone, archive, activate, and deactivate operations.
/// </summary>
[ApiController]
[Route("api/v1/admin/subscription-plans")]
[Authorize]
[Produces("application/json")]
public class AdminSubscriptionPlansController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminSubscriptionPlansController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>List subscription plans with search, filter, and pagination.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.SubscriptionsView)]
    [SwaggerOperation(Summary = "List subscription plans", Description = "Search and filter plans. Administrators can include archived plans.")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SubscriptionPlanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] SubscriptionPlanListQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchSubscriptionPlansQuery(query), cancellationToken);
        return Ok(ApiResponse<PagedResult<SubscriptionPlanDto>>.Ok(result));
    }

    /// <summary>Get a subscription plan by ID.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SubscriptionsView)]
    [SwaggerOperation(Summary = "Get subscription plan by ID")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSubscriptionPlanByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<SubscriptionPlanDto>.Ok(result));
    }

    /// <summary>Create a new subscription plan with billing options and feature limits.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.SubscriptionsCreate)]
    [SwaggerOperation(Summary = "Create subscription plan", Description = "Create unlimited plans with monthly/quarterly/semi-annual/annual/lifetime billing options.")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionPlanDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateSubscriptionPlanCommand(request, _currentUser.UserId?.ToString()), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SubscriptionPlanDto>.Ok(result, "Subscription plan created successfully."));
    }

    /// <summary>Update an existing subscription plan.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Update subscription plan")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubscriptionPlanDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSubscriptionPlanCommand(id, request, _currentUser.UserId?.ToString()), cancellationToken);
        return Ok(ApiResponse<SubscriptionPlanDto>.Ok(result, "Subscription plan updated successfully."));
    }

    /// <summary>Soft-delete a subscription plan.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Delete subscription plan", Description = "Soft-deletes the plan. Existing subscriptions are preserved.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSubscriptionPlanCommand(id, _currentUser.UserId?.ToString()), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Subscription plan deleted successfully."));
    }

    /// <summary>Clone an existing plan with a new plan code.</summary>
    [HttpPost("{id:guid}/clone")]
    [HasPermission(PermissionCodes.SubscriptionsCreate)]
    [SwaggerOperation(Summary = "Clone subscription plan", Description = "Creates a copy of the plan with Inactive status.")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Clone(Guid id, [FromBody] CloneSubscriptionPlanDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CloneSubscriptionPlanCommand(id, request, _currentUser.UserId?.ToString()), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SubscriptionPlanDto>.Ok(result, "Subscription plan cloned successfully."));
    }

    /// <summary>Activate a subscription plan.</summary>
    [HttpPost("{id:guid}/activate")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Activate subscription plan")]
    [ProducesResponseType(typeof(ApiResponse<PlanActionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ActivateSubscriptionPlanCommand(id, _currentUser.UserId?.ToString()), cancellationToken);
        return Ok(ApiResponse<PlanActionResponseDto>.Ok(result));
    }

    /// <summary>Deactivate a subscription plan.</summary>
    [HttpPost("{id:guid}/deactivate")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Deactivate subscription plan")]
    [ProducesResponseType(typeof(ApiResponse<PlanActionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateSubscriptionPlanCommand(id, _currentUser.UserId?.ToString()), cancellationToken);
        return Ok(ApiResponse<PlanActionResponseDto>.Ok(result));
    }

    /// <summary>Suspend a subscription plan.</summary>
    [HttpPost("{id:guid}/suspend")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Suspend subscription plan", Description = "Temporarily suspends the plan from new subscriptions.")]
    [ProducesResponseType(typeof(ApiResponse<PlanActionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SuspendSubscriptionPlanCommand(id, _currentUser.UserId?.ToString()), cancellationToken);
        return Ok(ApiResponse<PlanActionResponseDto>.Ok(result));
    }

    /// <summary>Archive a subscription plan.</summary>
    [HttpPost("{id:guid}/archive")]
    [HasPermission(PermissionCodes.SubscriptionsEdit)]
    [SwaggerOperation(Summary = "Archive subscription plan", Description = "Archives the plan. Hidden from default listings.")]
    [ProducesResponseType(typeof(ApiResponse<PlanActionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ArchiveSubscriptionPlanCommand(id, _currentUser.UserId?.ToString()), cancellationToken);
        return Ok(ApiResponse<PlanActionResponseDto>.Ok(result));
    }
}
