using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Features.Users.Commands;
using Khadamati.Application.Features.Users.Queries;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Administrator user management — CRUD, suspend, activate, role assignment.</summary>
[ApiController]
[Route("api/v1/admin/users")]
[Authorize]
[Produces("application/json")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminUsersController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.UsersView)]
    [SwaggerOperation(Summary = "Search users with filters and pagination")]
    public async Task<IActionResult> List([FromQuery] AdminUserListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<AdminUserListItemDto>>.Ok(await _mediator.Send(new SearchAdminUsersQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.UsersView)]
    [SwaggerOperation(Summary = "Get user details by ID")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<AdminUserDetailDto>.Ok(await _mediator.Send(new GetAdminUserByIdQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.UsersCreate)]
    [SwaggerOperation(Summary = "Create a new user (any role)")]
    public async Task<IActionResult> Create([FromBody] CreateAdminUserDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new CreateAdminUserCommand(request, adminId, _currentUser.IpAddress), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<AdminUserDetailDto>.Ok(result, "User created successfully."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.UsersEdit)]
    [SwaggerOperation(Summary = "Update user profile and status")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdminUserDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new UpdateAdminUserCommand(id, request, adminId), ct);
        return Ok(ApiResponse<AdminUserDetailDto>.Ok(result, "User updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.UsersDelete)]
    [SwaggerOperation(Summary = "Soft-delete a user")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        await _mediator.Send(new DeleteAdminUserCommand(id, adminId), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "User deleted successfully."));
    }

    [HttpPost("{id:guid}/suspend")]
    [HasPermission(PermissionCodes.UsersSuspend)]
    [SwaggerOperation(Summary = "Suspend a user account")]
    public async Task<IActionResult> Suspend(Guid id, [FromBody] SuspendUserDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new SuspendUserCommand(id, request, adminId), ct);
        return Ok(ApiResponse<UserActionResponseDto>.Ok(result));
    }

    [HttpPost("{id:guid}/activate")]
    [HasPermission(PermissionCodes.UsersEdit)]
    [SwaggerOperation(Summary = "Activate a user account")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new ActivateUserCommand(id, adminId), ct);
        return Ok(ApiResponse<UserActionResponseDto>.Ok(result));
    }

    [HttpPut("{id:guid}/roles")]
    [HasPermission(PermissionCodes.UsersEdit)]
    [SwaggerOperation(Summary = "Assign roles to a user")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignUserRolesDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new AssignUserRolesCommand(id, request, adminId), ct);
        return Ok(ApiResponse<AdminUserDetailDto>.Ok(result, "Roles assigned successfully."));
    }

    [HttpPost("{id:guid}/verify-email")]
    [HasPermission(PermissionCodes.UsersVerifyEmail)]
    [SwaggerOperation(Summary = "Manually verify user email")]
    public async Task<IActionResult> VerifyEmail(Guid id, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new AdminVerifyUserEmailCommand(id, adminId), ct);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }
}
