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
[Route("api/v1/admin/rbac")]
[Authorize]
[Produces("application/json")]
public class AdminRbacController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminRbacController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("roles")]
    [HasPermission(PermissionCodes.RolesView)]
    [SwaggerOperation(Summary = "List roles with permission counts")]
    public async Task<IActionResult> ListRoles(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<AdminRoleDto>>.Ok(await _mediator.Send(new ListRbacRolesQuery(), ct)));

    [HttpGet("roles/{roleId:guid}/permissions")]
    [HasPermission(PermissionCodes.RolesView)]
    [SwaggerOperation(Summary = "Get permission matrix for a role")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId, CancellationToken ct) =>
        Ok(ApiResponse<RolePermissionMatrixDto>.Ok(await _mediator.Send(new GetRolePermissionMatrixQuery(roleId), ct)));

    [HttpPut("roles/{roleId:guid}/permissions")]
    [HasPermission(PermissionCodes.RolesManage)]
    [SwaggerOperation(Summary = "Replace permissions assigned to a role")]
    public async Task<IActionResult> UpdateRolePermissions(Guid roleId, [FromBody] UpdateRolePermissionsDto request, CancellationToken ct) =>
        Ok(ApiResponse<RolePermissionMatrixDto>.Ok(
            await _mediator.Send(new UpdateRolePermissionsCommand(roleId, request, _currentUser.UserId?.ToString()), ct),
            "Role permissions updated."));
}
