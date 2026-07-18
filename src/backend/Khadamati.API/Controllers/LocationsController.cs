using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Features.Admin.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/locations")]
[AllowAnonymous]
[Produces("application/json")]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("regions")]
    public async Task<IActionResult> ListRegions(CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<RegionDto>>.Ok(await _mediator.Send(
            new ListRegionsQuery(new RegionListQueryDto { IsActive = true, Page = 1, PageSize = 100 }), ct)));

    [HttpGet("cities")]
    public async Task<IActionResult> ListCities([FromQuery] Guid? regionId, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<CityDto>>.Ok(await _mediator.Send(
            new ListCitiesQuery(new CityListQueryDto { RegionId = regionId, IsActive = true, Page = 1, PageSize = 500 }), ct)));
}
