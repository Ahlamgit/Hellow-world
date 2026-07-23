using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Features.Payments.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/admin/regions")]
[Authorize]
[Produces("application/json")]
public class AdminRegionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminRegionsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> List([FromQuery] RegionListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<RegionDto>>.Ok(await _mediator.Send(new ListRegionsQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<RegionDto>.Ok(await _mediator.Send(new GetRegionQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Create([FromBody] CreateRegionDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRegionCommand(request, _currentUser.UserId?.ToString()), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RegionDto>.Ok(result, "Region created."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRegionDto request, CancellationToken ct) =>
        Ok(ApiResponse<RegionDto>.Ok(
            await _mediator.Send(new UpdateRegionCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "Region updated."));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRegionCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Region deleted."));
    }
}

[ApiController]
[Route("api/v1/admin/cities")]
[Authorize]
[Produces("application/json")]
public class AdminCitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminCitiesController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> List([FromQuery] CityListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<CityDto>>.Ok(await _mediator.Send(new ListCitiesQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<CityDto>.Ok(await _mediator.Send(new GetCityQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Create([FromBody] CreateCityDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCityCommand(request, _currentUser.UserId?.ToString()), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CityDto>.Ok(result, "City created."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityDto request, CancellationToken ct) =>
        Ok(ApiResponse<CityDto>.Ok(
            await _mediator.Send(new UpdateCityCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "City updated."));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCityCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "City deleted."));
    }
}

[ApiController]
[Route("api/v1/webhooks/moyasar")]
[AllowAnonymous]
[Produces("application/json")]
public class MoyasarWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoyasarWebhookController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [SwaggerOperation(Summary = "Moyasar payment webhook", Description = "Confirms booking payments when Moyasar reports paid status.")]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        if (string.IsNullOrWhiteSpace(rawBody))
            return BadRequest(ApiResponse<object>.Fail("Empty webhook payload."));

        var payload = System.Text.Json.JsonSerializer.Deserialize<MoyasarWebhookDto>(
            rawBody,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (payload is null)
            return BadRequest(ApiResponse<object>.Fail("Invalid webhook payload."));

        var signature = Request.Headers["X-Moyasar-Signature"].FirstOrDefault()
            ?? Request.Headers["X-Webhook-Secret"].FirstOrDefault();
        var result = await _mediator.Send(new ProcessMoyasarWebhookCommand(payload, signature, rawBody), ct);
        return Ok(ApiResponse<PaymentWebhookResultDto>.Ok(result));
    }
}
