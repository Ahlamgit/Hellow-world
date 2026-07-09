using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Verification;
using Khadamati.Application.Features.Verification;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/verification/documents")]
[Authorize(Roles = "Craftsman,StoreOwner,Store")]
[Produces("application/json")]
public class VerificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public VerificationController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpPost]
    [SwaggerOperation(Summary = "Submit a verification document")]
    public async Task<IActionResult> Submit([FromBody] SubmitVerificationDocumentDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SubmitVerificationDocumentCommand(RequireUserId(), request), ct);
        return Ok(ApiResponse<VerificationDocumentDto>.Ok(result, "Verification document submitted."));
    }
}
