using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Features.Identity.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/sessions")]
[Authorize]
[Produces("application/json")]
public class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public SessionsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SessionsView)]
    [SwaggerOperation(Summary = "List active sessions for current user")]
    public async Task<IActionResult> GetMySessions(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var sessions = await _mediator.Send(new GetSessionsQuery(userId, _currentUser.SessionId), ct);
        return Ok(ApiResponse<IReadOnlyList<SessionDto>>.Ok(sessions));
    }

    [HttpGet("user/{userId:guid}")]
    [HasPermission(PermissionCodes.SessionsView)]
    [SwaggerOperation(Summary = "List active sessions for a user (admin)")]
    public async Task<IActionResult> GetUserSessions(Guid userId, CancellationToken ct)
    {
        var sessions = await _mediator.Send(new GetSessionsQuery(userId, null), ct);
        return Ok(ApiResponse<IReadOnlyList<SessionDto>>.Ok(sessions));
    }

    [HttpDelete("{sessionId:guid}")]
    [SwaggerOperation(Summary = "Revoke a session (current user or admin)")]
    public async Task<IActionResult> RevokeSession(Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var isAdmin = await _mediator.Send(new GetMyPermissionsQuery(userId), ct);
        var admin = isAdmin.Contains(PermissionCodes.SessionsRevoke, StringComparer.OrdinalIgnoreCase);
        await _mediator.Send(new RevokeSessionCommand(userId, sessionId, _currentUser.IpAddress, admin), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Session revoked."));
    }

    [HttpDelete("others")]
    [SwaggerOperation(Summary = "Logout all other devices")]
    public async Task<IActionResult> RevokeOtherSessions(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var sessionId = _currentUser.SessionId ?? throw new UnauthorizedException("Session ID required in X-Session-Id header.");
        await _mediator.Send(new RevokeAllOtherSessionsCommand(userId, sessionId, _currentUser.IpAddress), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "All other sessions revoked."));
    }

    [HttpDelete]
    [HasPermission(PermissionCodes.SessionsRevokeAll)]
    [SwaggerOperation(Summary = "Logout all devices")]
    public async Task<IActionResult> RevokeAllSessions(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        await _mediator.Send(new RevokeAllSessionsCommand(userId, _currentUser.IpAddress), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "All sessions revoked."));
    }
}

[ApiController]
[Route("api/v1/profile")]
[Authorize]
[Produces("application/json")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ProfileController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get current user profile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var profile = await _mediator.Send(new GetProfileQuery(userId), ct);
        return Ok(ApiResponse<ProfileDto>.Ok(profile));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Update current user profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var profile = await _mediator.Send(new UpdateProfileCommand(userId, request), ct);
        return Ok(ApiResponse<ProfileDto>.Ok(profile, "Profile updated."));
    }
}

[ApiController]
[Route("api/v1/login-history")]
[Authorize]
[Produces("application/json")]
public class LoginHistoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public LoginHistoryController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.LoginHistoryView)]
    [SwaggerOperation(Summary = "Get login history for current user")]
    public async Task<IActionResult> GetMyHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var history = await _mediator.Send(new GetLoginHistoryQuery(userId, page, pageSize), ct);
        return Ok(ApiResponse<PagedLoginHistoryDto>.Ok(history));
    }
}
