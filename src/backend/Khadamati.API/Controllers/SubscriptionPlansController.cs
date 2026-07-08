using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Features.Subscriptions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>
/// Public subscription plan catalog for Customers, Craftsmen, and Stores.
/// </summary>
[ApiController]
[Route("api/v1/subscription-plans")]
[Produces("application/json")]
public class SubscriptionPlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionPlansController(IMediator mediator) => _mediator = mediator;

    /// <summary>List active subscription plans available for subscription.</summary>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "List active subscription plans", Description = "Returns active plans ordered by display priority. Optionally filter by target role.")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SubscriptionPlanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? targetRole, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPublicSubscriptionPlansQuery(targetRole), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SubscriptionPlanDto>>.Ok(result));
    }

    /// <summary>Get an active subscription plan by ID.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get subscription plan by ID")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSubscriptionPlanByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<SubscriptionPlanDto>.Ok(result));
    }
}
