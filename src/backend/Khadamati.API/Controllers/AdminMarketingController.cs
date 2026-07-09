using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/admin/coupons")]
[Authorize]
[Produces("application/json")]
public class AdminCouponsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminCouponsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> List([FromQuery] CouponListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<CouponDto>>.Ok(await _mediator.Send(new ListCouponsQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<CouponDto>.Ok(await _mediator.Send(new GetCouponQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Create([FromBody] CreateCouponDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCouponCommand(request, _currentUser.UserId?.ToString()), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CouponDto>.Ok(result, "Coupon created."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponDto request, CancellationToken ct) =>
        Ok(ApiResponse<CouponDto>.Ok(
            await _mediator.Send(new UpdateCouponCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "Coupon updated."));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCouponCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Coupon deleted."));
    }
}

[ApiController]
[Route("api/v1/admin/advertisements")]
[Authorize]
[Produces("application/json")]
public class AdminAdvertisementsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminAdvertisementsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AdvertisementsManage)]
    public async Task<IActionResult> List([FromQuery] AdvertisementListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<AdminAdvertisementDto>>.Ok(await _mediator.Send(new ListAdvertisementsQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.AdvertisementsManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<AdminAdvertisementDto>.Ok(await _mediator.Send(new GetAdvertisementQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.AdvertisementsManage)]
    public async Task<IActionResult> Create([FromBody] CreateAdvertisementDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateAdvertisementCommand(request, _currentUser.UserId?.ToString()), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<AdminAdvertisementDto>.Ok(result, "Advertisement created."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AdvertisementsManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdvertisementDto request, CancellationToken ct) =>
        Ok(ApiResponse<AdminAdvertisementDto>.Ok(
            await _mediator.Send(new UpdateAdvertisementCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "Advertisement updated."));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AdvertisementsManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAdvertisementCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Advertisement deleted."));
    }
}
