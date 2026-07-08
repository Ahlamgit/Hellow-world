using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Craftsman;
using Khadamati.Application.Features.Craftsman;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Craftsman self-service portal API.</summary>
[ApiController]
[Route("api/v1/me/craftsman")]
[Authorize(Roles = "Craftsman")]
[Produces("application/json")]
public class MeCraftsmanController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public MeCraftsmanController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet]
    [SwaggerOperation(Summary = "Get craftsman profile with services and working hours")]
    public async Task<IActionResult> GetProfile(CancellationToken ct) =>
        Ok(ApiResponse<CraftsmanProfileDto>.Ok(await _mediator.Send(new GetMyCraftsmanProfileQuery(RequireUserId()), ct)));

    [HttpPut]
    [SwaggerOperation(Summary = "Update craftsman profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCraftsmanProfileDto request, CancellationToken ct) =>
        Ok(ApiResponse<CraftsmanProfileDto>.Ok(await _mediator.Send(new UpdateMyCraftsmanProfileCommand(RequireUserId(), request), ct)));

    [HttpPost("services")]
    [SwaggerOperation(Summary = "Add or update an offered service")]
    public async Task<IActionResult> UpsertService([FromBody] UpsertCraftsmanServiceDto request, CancellationToken ct) =>
        Ok(ApiResponse<CraftsmanServiceDto>.Ok(await _mediator.Send(new UpsertCraftsmanServiceCommand(RequireUserId(), request), ct)));

    [HttpDelete("services/{id:guid}")]
    public async Task<IActionResult> DeleteService(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCraftsmanServiceCommand(RequireUserId(), id), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Service removed."));
    }

    [HttpPost("working-hours")]
    [SwaggerOperation(Summary = "Add or update working hours for a day")]
    public async Task<IActionResult> UpsertWorkingHour([FromBody] UpsertWorkingHourDto request, CancellationToken ct) =>
        Ok(ApiResponse<CraftsmanWorkingHourDto>.Ok(await _mediator.Send(new UpsertCraftsmanWorkingHourCommand(RequireUserId(), request), ct)));

    [HttpDelete("working-hours/{id:guid}")]
    public async Task<IActionResult> DeleteWorkingHour(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCraftsmanWorkingHourCommand(RequireUserId(), id), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Working hours removed."));
    }
}
