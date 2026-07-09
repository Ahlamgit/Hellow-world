using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Support;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/admin/complaints")]
[Authorize]
[Produces("application/json")]
public class AdminComplaintsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissions;

    public AdminComplaintsController(IMediator mediator, ICurrentUserService currentUser, IPermissionService permissions)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _permissions = permissions;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get complaint details")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersView, ct);
        return Ok(ApiResponse<AdminComplaintDetailDto>.Ok(await _mediator.Send(new GetAdminComplaintQuery(id), ct)));
    }

    [HttpPost("{id:guid}/resolve")]
    [SwaggerOperation(Summary = "Resolve a complaint")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveComplaintDto request, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersEdit, ct);
        var result = await _mediator.Send(new ResolveAdminComplaintCommand(id, request, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<AdminComplaintDetailDto>.Ok(result, "Complaint resolved."));
    }
}

[ApiController]
[Route("api/v1/admin/support-tickets")]
[Authorize]
[Produces("application/json")]
public class AdminSupportTicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissions;

    public AdminSupportTicketsController(IMediator mediator, ICurrentUserService currentUser, IPermissionService permissions)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _permissions = permissions;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get support ticket details")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersView, ct);
        return Ok(ApiResponse<AdminSupportTicketDetailDto>.Ok(await _mediator.Send(new GetAdminSupportTicketQuery(id), ct)));
    }

    [HttpPost("{id:guid}/close")]
    [SwaggerOperation(Summary = "Close a support ticket")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersEdit, ct);
        var result = await _mediator.Send(new CloseAdminSupportTicketCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<AdminSupportTicketDetailDto>.Ok(result, "Support ticket closed."));
    }
}

[ApiController]
[Route("api/v1/admin/payments")]
[Authorize]
[Produces("application/json")]
public class AdminPaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissions;

    public AdminPaymentsController(IMediator mediator, ICurrentUserService currentUser, IPermissionService permissions)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _permissions = permissions;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get payment details")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.PaymentsView, ct);
        return Ok(ApiResponse<AdminPaymentDetailDto>.Ok(await _mediator.Send(new GetAdminPaymentQuery(id), ct)));
    }
}
