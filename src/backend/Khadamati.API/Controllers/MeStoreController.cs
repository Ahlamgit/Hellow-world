using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Store;
using Khadamati.Application.Features.Store;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Store owner self-service portal API.</summary>
[ApiController]
[Route("api/v1/me/store")]
[Authorize(Roles = "StoreOwner,StoreEmployee")]
[Produces("application/json")]
public class MeStoreController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public MeStoreController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet]
    [SwaggerOperation(Summary = "Get store profile with products")]
    public async Task<IActionResult> GetProfile(CancellationToken ct) =>
        Ok(ApiResponse<StoreProfileDto>.Ok(await _mediator.Send(new GetMyStoreProfileQuery(RequireUserId()), ct)));

    [HttpPut]
    [SwaggerOperation(Summary = "Update store profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateStoreProfileDto request, CancellationToken ct) =>
        Ok(ApiResponse<StoreProfileDto>.Ok(await _mediator.Send(new UpdateMyStoreProfileCommand(RequireUserId(), request), ct)));

    [HttpPost("products")]
    [SwaggerOperation(Summary = "Create a store product")]
    public async Task<IActionResult> CreateProduct([FromBody] UpsertStoreProductDto request, CancellationToken ct) =>
        Ok(ApiResponse<StoreProductDto>.Ok(await _mediator.Send(new UpsertStoreProductCommand(RequireUserId(), request, null), ct)));

    [HttpPut("products/{id:guid}")]
    [SwaggerOperation(Summary = "Update a store product")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpsertStoreProductDto request, CancellationToken ct) =>
        Ok(ApiResponse<StoreProductDto>.Ok(await _mediator.Send(new UpsertStoreProductCommand(RequireUserId(), request, id), ct)));

    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteStoreProductCommand(RequireUserId(), id), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Product removed."));
    }
}
