using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Verification;
using Khadamati.Application.Features.Verification;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/admin/verification-documents")]
[Authorize]
[Produces("application/json")]
public class AdminVerificationDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissions;

    public AdminVerificationDocumentsController(
        IMediator mediator, ICurrentUserService currentUser, IPermissionService permissions)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _permissions = permissions;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet]
    [SwaggerOperation(Summary = "List verification documents")]
    public async Task<IActionResult> List([FromQuery] VerificationDocumentListQueryDto query, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersView, ct);
        return Ok(ApiResponse<PagedResult<VerificationDocumentDto>>.Ok(
            await _mediator.Send(new ListVerificationDocumentsQuery(query), ct)));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get verification document details")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersView, ct);
        return Ok(ApiResponse<VerificationDocumentDto>.Ok(await _mediator.Send(new GetVerificationDocumentQuery(id), ct)));
    }

    [HttpPost("{id:guid}/approve")]
    [SwaggerOperation(Summary = "Approve a verification document")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersEdit, ct);
        var result = await _mediator.Send(new ApproveVerificationDocumentCommand(id, RequireUserId()), ct);
        return Ok(ApiResponse<VerificationDocumentDto>.Ok(result, "Verification document approved."));
    }

    [HttpPost("{id:guid}/reject")]
    [SwaggerOperation(Summary = "Reject a verification document")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectVerificationDocumentDto request, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.UsersEdit, ct);
        var result = await _mediator.Send(new RejectVerificationDocumentCommand(id, RequireUserId(), request), ct);
        return Ok(ApiResponse<VerificationDocumentDto>.Ok(result, "Verification document rejected."));
    }
}
