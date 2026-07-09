using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/admin/settings")]
[Authorize]
[Produces("application/json")]
public class AdminSettingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminSettingsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "List all system settings")]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<SystemSettingDto>>.Ok(await _mediator.Send(new ListSystemSettingsQuery(), ct)));

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Update a system setting value")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSystemSettingDto request, CancellationToken ct) =>
        Ok(ApiResponse<SystemSettingDto>.Ok(
            await _mediator.Send(new UpdateSystemSettingCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "Setting updated."));
}
